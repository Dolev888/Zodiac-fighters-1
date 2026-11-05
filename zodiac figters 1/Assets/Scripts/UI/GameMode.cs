// One flag that says how the next match is played. Set by the main menu's Play panel, read by everything else.
//   Versus = false -> Player vs CPU (the normal game)
//   Versus = true  -> Player vs Player on the same computer (used for the trailer)
// It lives in its own file so none of the other scripts need to be edited to add it.
// Later, the script that gives the opponent its input (FighterRole) can check GameMode.Versus.
public static class GameMode
{
    public static bool Versus = false;
}