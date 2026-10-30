using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Playtest log. Writes one line per event to a plain text file and, when the game closes, a summary
/// block for the run (one game launch = one run). Nothing needs to be added to a scene: it creates
/// itself when the game starts and survives scene changes.
///
/// Other scripts talk to it with the static calls at the bottom, for example
///     CollectedDataAnalysis.PlayerPick(0);
/// They do nothing if the logger doesn't exist, so a missing logger can never break the game.
///
/// What it records (for the playtest survey):
///  1  character picks (the player's fighter and the opponent)
///  2  wins with each character, and 4: AI wins with each character
///  3  how long the controls list is open during a fight (pause menu)
///  5  attack use (ground / air, normal / special) for the player and for the AI
///  6  attack hits (hit rate = hits landed / attacks used)
///  7  falls from the arena
///  8  duration of every fight (paused time is not counted)
///  9  health left for the winner at the end of the fight
///
/// The file is created next to the game's .exe in a folder called "Playtest Log"
/// (the project folder in the Editor). If that folder can't be written to, it falls back to Unity's
/// persistent data folder. The path is printed in the Console when the game starts.
/// Existing lines are never touched, so data accumulates across sessions.
/// </summary>
public class CollectedDataAnalysis : MonoBehaviour
{
    public static CollectedDataAnalysis Instance { get; private set; }

    // same order as MatchSetup.characterPrefabs and the numbers the selection buttons use. Add the new character here.
    private static readonly string[] CharacterNames = { "Aries", "Scorpio", "Taurus" };

    private const string RunNumberPrefKey = "CollectedDataAnalysis.RunNumber";
    private const string LogFolderName = "Playtest Log";
    private const string LogFileName = "playtest_log.txt";
    private const string NewLine = "\r\n";   // same on every OS
    private static readonly Encoding FileEncoding = new UTF8Encoding(false);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;

        var go = new GameObject(nameof(CollectedDataAnalysis));
        DontDestroyOnLoad(go);
        go.AddComponent<CollectedDataAnalysis>();
    }

    // ---------------------------------------------------------------- run state
    private int runNumber;                 // 0 until the first event of this launch
    private bool summaryWritten;
    private DateTime sessionStart;
    private readonly HashSet<string> loggedScenes = new HashSet<string>();
    private string workingFolder;          // the folder that worked for writing

    // ---------------------------------------------------------------- current match
    private bool matchActive;
    private float matchTime;               // scaled time, so the pause menu (timeScale 0) isn't counted
    private string matchScene;
    private int playerIndex, enemyIndex;
    private FighterDamage playerDamage, enemyDamage;

    // ---------------------------------------------------------------- tallies for the summary
    private class WinStat { public int matches; public int wins; }

    private readonly Dictionary<string, int> playerPicks = new Dictionary<string, int>();
    private readonly Dictionary<string, int> enemyPicks = new Dictionary<string, int>();

    private int matchesStarted, matchesFinished, matchesAbandoned;
    private int playerWins, aiWins;
    private readonly Dictionary<string, WinStat> winStats = new Dictionary<string, WinStat>();   // "Aries (player)", "Scorpio (AI)"
    private readonly List<string> matchLines = new List<string>();

    private readonly Dictionary<string, int> attackCounts = new Dictionary<string, int>();       // "Player ground normal"
    private int attacksByPlayer, attacksByAI;
    private int hitsOnPlayer, hitsOnAI;                // hits that landed ON the player / ON the AI

    private int fallsPlayer, fallsAI;

    private int controlsOpenCount;
    private float controlsOpenSeconds;
    private float controlsOpenedAt = -1f;

    // ================================================================ unity
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        sessionStart = DateTime.Now;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("CollectedDataAnalysis writing to " + Path.Combine(PrimaryFolder(), LogFileName));
    }

    private void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (matchActive) matchTime += Time.deltaTime;
    }

    private void OnApplicationQuit()
    {
        // covers every way of closing the game; the guard makes sure the summary is written once
        if (matchActive) AbandonMatch();
        WriteRunSummary();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // the first load of each scene is logged once
        if (loggedScenes.Add(scene.name))
            Log("SCENE_LOADED", "Scene: " + scene.name, false);

        // leaving the match scene while a match is running (Main Menu button from the pause menu, for example)
        if (matchActive && scene.name != matchScene) AbandonMatch();
    }

    // ================================================================ static calls (use these from other scripts)
    public static void PlayerPick(int characterIndex) { if (Instance != null) Instance.DoPick(true, characterIndex); }
    public static void EnemyPick(int characterIndex) { if (Instance != null) Instance.DoPick(false, characterIndex); }

    public static void MatchStart(int playerCharacter, int enemyCharacter, FighterDamage player, FighterDamage enemy)
    {
        if (Instance != null) Instance.DoMatchStart(playerCharacter, enemyCharacter, player, enemy);
    }

    public static void MatchEnd(bool playerWon) { if (Instance != null) Instance.DoMatchEnd(playerWon); }

    // isPlayer = who attacks (true = the player, false = the AI)
    public static void Attack(bool isPlayer, bool air, bool special)
    {
        if (Instance != null) Instance.DoAttack(isPlayer, air, special);
    }

    // a dash attack (forward dash counts as an attack, a back dash is only recorded)
    public static void Dash(bool isPlayer, bool air, bool back)
    {
        if (Instance != null) Instance.DoDash(isPlayer, air, back);
    }

    // victimIsPlayer = who was hit
    public static void Hit(bool victimIsPlayer, float damage)
    {
        if (Instance != null) Instance.DoHit(victimIsPlayer, damage);
    }

    // isPlayer = who fell off the arena
    public static void Fall(bool isPlayer) { if (Instance != null) Instance.DoFall(isPlayer); }

    public static void ControlsOpened() { if (Instance != null) Instance.DoControlsOpened(); }
    public static void ControlsClosed() { if (Instance != null) Instance.DoControlsClosed(); }

    // ================================================================ the work
    public static string CharacterName(int index)
    {
        return index >= 0 && index < CharacterNames.Length ? CharacterNames[index] : "Character" + index;
    }

    private static string Side(bool isPlayer) { return isPlayer ? "Player" : "AI"; }

    private static void Add(Dictionary<string, int> dict, string key)
    {
        int n;
        dict[key] = dict.TryGetValue(key, out n) ? n + 1 : 1;
    }

    private void DoPick(bool forPlayer, int characterIndex)
    {
        string name = CharacterName(characterIndex);
        Add(forPlayer ? playerPicks : enemyPicks, name);
        Log(forPlayer ? "PLAYER_PICK" : "ENEMY_PICK", "Character: " + name);
    }

    private void DoMatchStart(int playerCharacter, int enemyCharacter, FighterDamage player, FighterDamage enemy)
    {
        // a new match starting means the previous one (if any) never finished, for example after Rematch mid-fight
        if (matchActive) AbandonMatch();

        matchActive = true;
        matchTime = 0f;
        matchScene = SceneManager.GetActiveScene().name;
        playerIndex = playerCharacter;
        enemyIndex = enemyCharacter;
        playerDamage = player;
        enemyDamage = enemy;
        matchesStarted++;

        Log("MATCH_START", "Player: " + CharacterName(playerCharacter) + " | AI: " + CharacterName(enemyCharacter));
    }

    private void DoMatchEnd(bool playerWon)
    {
        if (!matchActive) return;
        matchActive = false;
        matchesFinished++;

        string playerName = CharacterName(playerIndex);
        string aiName = CharacterName(enemyIndex);

        // health left for the winner, the same number the health box shows (100 -> 0)
        FighterDamage winner = playerWon ? playerDamage : enemyDamage;
        string healthLeft = "?";
        if (winner != null)
            healthLeft = Mathf.Max(0, Mathf.RoundToInt(winner.MaxDamage - winner.DamagePercentage)) + "%";

        if (playerWon) playerWins++; else aiWins++;
        GetStat(playerName + " (player)").matches++;
        GetStat(aiName + " (AI)").matches++;
        GetStat((playerWon ? playerName + " (player)" : aiName + " (AI)")).wins++;

        string winnerText = playerWon ? "Player (" + playerName + ")" : "AI (" + aiName + ")";
        string duration = FormatTime(matchTime);

        matchLines.Add("Match " + matchesFinished + ": " + playerName + " vs " + aiName + " | winner " + winnerText
            + " | " + duration + " | winner health left " + healthLeft);

        Log("MATCH_END", "Winner: " + winnerText + " | Player: " + playerName + " | AI: " + aiName
            + " | Duration: " + duration + " | Winner health left: " + healthLeft);
    }

    private void AbandonMatch()
    {
        matchActive = false;
        matchesAbandoned++;
        Log("MATCH_ABANDONED", "Player: " + CharacterName(playerIndex) + " | AI: " + CharacterName(enemyIndex)
            + " | Duration: " + FormatTime(matchTime));
    }

    private WinStat GetStat(string key)
    {
        WinStat s;
        if (!winStats.TryGetValue(key, out s))
        {
            s = new WinStat();
            winStats[key] = s;
        }
        return s;
    }

    private void DoAttack(bool isPlayer, bool air, bool special)
    {
        RecordAttack(isPlayer, (air ? "air " : "ground ") + (special ? "special" : "normal"), true);
    }

    private void DoDash(bool isPlayer, bool air, bool back)
    {
        // a forward dash is an attack and counts in the attack totals; a back dash is only recorded
        RecordAttack(isPlayer, (air ? "air " : "ground ") + (back ? "back dash" : "dash"), !back);
    }

    private void RecordAttack(bool isPlayer, string type, bool countInTotals)
    {
        Add(attackCounts, Side(isPlayer) + " " + type);
        if (countInTotals)
        {
            if (isPlayer) attacksByPlayer++; else attacksByAI++;
        }
        Log("ATTACK", "Side: " + Side(isPlayer) + " | Type: " + type);
    }

    private void DoHit(bool victimIsPlayer, float damage)
    {
        if (victimIsPlayer) hitsOnPlayer++; else hitsOnAI++;
        // "Side" is who LANDED the hit, "Victim" is who was hit
        Log("HIT", "Side: " + Side(!victimIsPlayer) + " | Victim: " + Side(victimIsPlayer) + " | Damage: " + damage);
    }

    private void DoFall(bool isPlayer)
    {
        if (isPlayer) fallsPlayer++; else fallsAI++;
        Log("FALL", "Who: " + Side(isPlayer));
    }

    private void DoControlsOpened()
    {
        if (controlsOpenedAt >= 0f) return;   // already open
        controlsOpenedAt = Time.realtimeSinceStartup;
        controlsOpenCount++;
        Log("CONTROLS_OPEN");
    }

    private void DoControlsClosed()
    {
        if (controlsOpenedAt < 0f) return;
        float seconds = Time.realtimeSinceStartup - controlsOpenedAt;
        controlsOpenedAt = -1f;
        controlsOpenSeconds += seconds;
        Log("CONTROLS_CLOSE", "Seconds open: " + seconds.ToString("0.0"));
    }

    // ================================================================ writing
    private void StartRunIfNeeded()
    {
        if (runNumber != 0) return;

        runNumber = PlayerPrefs.GetInt(RunNumberPrefKey, 0) + 1;
        PlayerPrefs.SetInt(RunNumberPrefKey, runNumber);
        PlayerPrefs.Save();
    }

    // "time | Run n | EVENT [| detail] | Scene: name"
    private void Log(string eventName, string detail = null, bool includeScene = true)
    {
        StartRunIfNeeded();

        string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | Run " + runNumber + " | " + eventName;
        if (detail != null) line += " | " + detail;
        if (includeScene) line += " | Scene: " + SceneManager.GetActiveScene().name;

        WriteLine(line);
    }

    private void WriteRunSummary()
    {
        if (summaryWritten) return;
        summaryWritten = true;

        if (runNumber == 0) return;   // nothing happened this launch

        var sb = new StringBuilder();
        sb.Append("=== Run " + runNumber + " summary (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ") ===").Append(NewLine);
        sb.Append("Session length: " + FormatTime((float)(DateTime.Now - sessionStart).TotalSeconds)).Append(NewLine);

        sb.Append("Matches: " + matchesStarted + " started, " + matchesFinished + " finished, " + matchesAbandoned + " abandoned").Append(NewLine);
        sb.Append("Wins: player " + playerWins + ", AI " + aiWins).Append(NewLine);

        sb.Append("Player picks: " + Breakdown(playerPicks)).Append(NewLine);
        sb.Append("Opponent picks: " + Breakdown(enemyPicks)).Append(NewLine);

        sb.Append("Wins per character:").Append(NewLine);
        if (winStats.Count == 0) sb.Append("  (no finished matches)").Append(NewLine);
        foreach (var pair in winStats)
            sb.Append("  - " + pair.Key + ": " + pair.Value.wins + " of " + pair.Value.matches + " (" + Pct(pair.Value.wins, pair.Value.matches) + ")").Append(NewLine);

        sb.Append("Attacks used: player " + attacksByPlayer + ", AI " + attacksByAI).Append(NewLine);
        foreach (var pair in attackCounts)
            sb.Append("  - " + pair.Key + ": " + pair.Value).Append(NewLine);

        sb.Append("Hit rate (hits landed / attacks used): player " + hitsOnAI + "/" + attacksByPlayer + " (" + Pct(hitsOnAI, attacksByPlayer)
            + "), AI " + hitsOnPlayer + "/" + attacksByAI + " (" + Pct(hitsOnPlayer, attacksByAI) + ")").Append(NewLine);

        sb.Append("Falls from the arena: player " + fallsPlayer + ", AI " + fallsAI).Append(NewLine);
        sb.Append("Controls list opened: " + controlsOpenCount + " times, " + controlsOpenSeconds.ToString("0.0") + " seconds in total").Append(NewLine);

        sb.Append("Fights:").Append(NewLine);
        if (matchLines.Count == 0) sb.Append("  (none finished)").Append(NewLine);
        foreach (string m in matchLines)
            sb.Append("  - " + m).Append(NewLine);

        sb.Append("===================================");

        WriteLine(sb.ToString());
    }

    private static string Breakdown(Dictionary<string, int> dict)
    {
        if (dict.Count == 0) return "none";

        int total = 0;
        foreach (var pair in dict) total += pair.Value;

        var parts = new List<string>();
        foreach (var pair in dict)
            parts.Add(pair.Key + " " + pair.Value + " (" + Pct(pair.Value, total) + ")");
        return string.Join(", ", parts);
    }

    private static string Pct(int part, int whole)
    {
        if (whole <= 0) return "n/a";
        return (100f * part / whole).ToString("0") + "%";
    }

    private static string FormatTime(float seconds)
    {
        var t = TimeSpan.FromSeconds(seconds);
        return ((int)t.TotalHours).ToString("00") + ":" + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00");
    }

    // ================================================================ file access
    // the folder with the game's .exe (the project folder in the Editor)
    private static string GameFolder()
    {
        // Application.dataPath is "<game>_Data" on Windows and "<project>/Assets" in the Editor
        var folder = Directory.GetParent(Application.dataPath);
        if (Application.platform == RuntimePlatform.OSXPlayer && folder.Parent != null)
            folder = folder.Parent;
        return folder.FullName;
    }

    private static string PrimaryFolder() { return Path.Combine(GameFolder(), LogFolderName); }
    private static string FallbackFolder() { return Path.Combine(Application.persistentDataPath, LogFolderName); }

    private void WriteLine(string text)
    {
        // use the folder that worked before; otherwise try the game folder first, then the fallback
        string[] candidates = workingFolder != null
            ? new[] { workingFolder }
            : new[] { PrimaryFolder(), FallbackFolder() };

        foreach (string folder in candidates)
        {
            try
            {
                Directory.CreateDirectory(folder);   // does nothing if it already exists
                File.AppendAllText(Path.Combine(folder, LogFileName), text + NewLine, FileEncoding);

                if (workingFolder == null)
                {
                    workingFolder = folder;
                    Debug.Log("CollectedDataAnalysis is writing to " + Path.Combine(folder, LogFileName));
                }
                return;
            }
            catch (Exception e)   // a read-only location throws UnauthorizedAccessException, not just IOException
            {
                Debug.LogWarning("CollectedDataAnalysis could not write to " + folder + ": " + e.Message);
            }
        }
    }
}