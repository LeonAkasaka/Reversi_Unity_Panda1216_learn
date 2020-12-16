using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mypage_Controller : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        var blackwin = PlayerPrefs.GetInt(SaveData_Manager.BLACK_WIN, 0);

        var esaplus = PlayerPrefs.GetInt(SaveData_Manager.KEY_PANDA_ESA, 0);

        Debug.Log("esaplus" + esaplus);
        Debug.Log("blackwin" + blackwin);

        SoundManager.Instance.BGM_Game_Main_Play();
    }
}
