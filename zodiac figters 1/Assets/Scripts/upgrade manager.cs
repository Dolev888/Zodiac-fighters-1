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
    [SerializeField] private GameObject[] _fireBack;
    [SerializeField] private GameObject[] _airBack;
    [SerializeField] private GameObject[] _waterBack;
    [SerializeField] private GameObject[] _earthBack;
    [SerializeField] private playermain _playerPmain;
    private int[] upgradeNum = new int[3];


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_playerPmain != null) 
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

    }
    public void Upgrade3Push()
    {

    }
    private void upgradeSetUp()
    {
        playermain.TYPE type = _playerPmain.Type;
        switch (type)
        {
            case (playermain.TYPE.FIRE):
                for (int i = 0; i < _firePalet.Length; i++)
                {
                    _upgradeButton1[i].GetComponent<Image>().color = _firePalet[i];
                    _upgradeButton2[i].GetComponent<Image>().color = _firePalet[i];
                    _upgradeButton3[i].GetComponent<Image>().color = _firePalet[i];
                    _fireBack[i].SetActive(true);
                }  
                break;
            case (playermain.TYPE.EARTH):
                for (int i = 0; i < _firePalet.Length; i++)
                {
                    _upgradeButton1[i].GetComponent<Image>().color = _earthPalet[i];
                    _upgradeButton2[i].GetComponent<Image>().color = _earthPalet[i];
                    _upgradeButton3[i].GetComponent<Image>().color = _earthPalet[i];
                    _waterBack[i].SetActive(true);
                }
                break;
            case (playermain.TYPE.WATER):
                for (int i = 0; i < _firePalet.Length; i++)
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
}
