using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class upgrademanager : MonoBehaviour
{
    [SerializeField] private Color[] _firePalet;
    [SerializeField] private Color[] _earthPalet;
    [SerializeField] private Color[] _waterPalet;
    [SerializeField] private GameObject[] _upgradeButton1;
    [SerializeField] private GameObject[] _upgradeButton2;
    [SerializeField] private GameObject[] _upgradeButton3;
    [SerializeField] private TextMeshPro[] _titleList;
    [SerializeField] private TextMeshPro[] _TEXTlIST;
    [SerializeField] private GameObject[] _fireBack;
    [SerializeField] private GameObject[] _airBack;
    [SerializeField] private GameObject[] _waterBack;
    [SerializeField] private GameObject[] _earthBack;
    [SerializeField] private playermain _playerPmain;
    private int[] upgradeNum = new int[3];
    [SerializeField] private upgradedeta[] _airisList;
    [SerializeField] private upgradedeta[] _scorpioList;
    [SerializeField] private upgradedeta[] _taurusList;
    private HashSet<upgradedeta> _List;
    [SerializeField] private MatchSetup _match;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerPmain = _match.player.main;
        if (_playerPmain != null) 
        upgradeSetUp();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Upgrade1Push()
    {
        Debug.Log("1");
        _playerPmain.pattack._upgradeBoolList[upgradeNum[0]]=true;
    }
    public void Upgrade2Push()
    {
        _playerPmain.pattack._upgradeBoolList[upgradeNum[1]] = true;
    }
    public void Upgrade3Push()
    {
        _playerPmain.pattack._upgradeBoolList[upgradeNum[2]] = true;
    }
    private void upgradeSetUp()
    {
        playermain.TYPE type = _playerPmain.Type;
        switch (type)
        {
            case (playermain.TYPE.FIRE):
                for (int i = 0; i < _airisList.Length; i++)
                {
                    _List.Add(_airisList[i]);
                }                
                for (int i = 0; i < _firePalet.Length; i++)
                {
                   
                    _upgradeButton1[i].GetComponent<Image>().color = _firePalet[i];
                    _upgradeButton2[i].GetComponent<Image>().color = _firePalet[i];
                    _upgradeButton3[i].GetComponent<Image>().color = _firePalet[i];
                    _fireBack[i].SetActive(true);
                }
                for (int i = 0; i < 3; i++)
                {
                    setText(i);
                }
                break;
            case (playermain.TYPE.EARTH):
                for (int i = 0; i < _taurusList.Length; i++)
                {
                    _List.Add(_taurusList[i]);
                }
                
                for (int i = 0; i < _earthPalet.Length; i++)
                {
                    
                    _upgradeButton1[i].GetComponent<Image>().color = _earthPalet[i];
                    _upgradeButton2[i].GetComponent<Image>().color = _earthPalet[i];
                    _upgradeButton3[i].GetComponent<Image>().color = _earthPalet[i];
                    _waterBack[i].SetActive(true);
                }
                break;
            case (playermain.TYPE.WATER):
                for (int i = 0; i < _scorpioList.Length; i++)
                {
                    _List.Add(_scorpioList[i]);
                }
                
                for (int i = 0; i < _waterPalet.Length; i++)
                {
                    
                    _upgradeButton1[i].GetComponent<Image>().color = _waterPalet[i];
                    _upgradeButton2[i].GetComponent<Image>().color = _waterPalet[i];
                    _upgradeButton3[i].GetComponent<Image>().color = _waterPalet[i];
                    _earthBack[i].SetActive(true);
                }
                
                break;
            case (playermain.TYPE.AIR):
                break;
        }
        
    }
    private void setText(int num)
    {
        int rund = Random.Range(0, _List.Count);
        upgradedeta[] list = _List.ToArray();
        _titleList[num].text = list[rund]._title;
        _TEXTlIST[num].text = list[rund]._text;
        upgradeNum[num] = list[rund]._effectNum;
    }
}
