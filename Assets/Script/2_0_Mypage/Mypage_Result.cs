﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マイページのリザルト画面を出すクラス
/// </summary>
public class Mypage_Result : MonoBehaviour
{
    /// <summary>
    /// パンダのスプライト5段階分
    /// </summary>
    [SerializeField]
    private Sprite[] _Result_Panda_sprites = new Sprite[5];

    [SerializeField]
    private Image _Result_Panda;

   
    /// <summary>
    /// PlayerPrefのスコアの引き継ぎ
    /// </summary>
    private int Black_Win;

    private int White_Win;

    private float Black_Percent;
   
    private float White_Percent;

    private int BW_Result;

    [SerializeField]
    private Text _Text_Hitokoto;

    [SerializeField]
    private Text _Text_White;

    [SerializeField]
    private Text _Text_Black;

    [SerializeField]
    private Text _Text_BWP;

    [SerializeField]
    GameObject Rerult_Panel;


    // Start is called before the first frame update
    void Start()
    {
        //キーの呼び出し 
       var Black_Win = (float)PlayerPrefs.GetInt("BLACKWIN", 0);

       var White_Win =  PlayerPrefs.GetInt("WHITEWIN", 0);

        //結果を全体を10とした割合で表示する
        var result = (float)10 / ((float)Black_Win + (float)White_Win);


        Debug.Log(result);
       

        Black_Percent = result * (int)Black_Win;
        White_Percent = result * (int)White_Win;

        var strblack = Black_Win.ToString();
        var strwhite = White_Win.ToString();

        //勝利数の表示
        _Text_Black.text = "黒で勝利数："+strblack;
        _Text_White.text = "白で勝利数：" + strwhite;

        //黒白勝率の比率
        var bp1 = (int)Black_Percent;
        var wp1 = (int)White_Percent;

        var bp = bp1.ToString();
        var wp = wp1.ToString();

        _Text_BWP.text = "黒:  " + bp + "白:  " + wp;

        Display_Text();
        Display_Image();

    }
        
    /// <summary>
    /// 表示テキスト（ひとこと）
    /// </summary>
    public void Display_Text()
    {

        if (Black_Percent >= 9)
        {

            var str = "もういっそヒグマにしてほしい";

            _Text_Hitokoto.text = str;


        }
        else if (Black_Percent >= 7)
        {
            var str = "ぎりパンダ";

            _Text_Hitokoto.text = str;

        }
        else if (Black_Percent <= 3)
        {
            var str = "割と白めのパンダ";

            _Text_Hitokoto.text = str;


        }
        else if (Black_Percent <= 1)
        {
            var str = "もはや白クマ";
            _Text_Hitokoto.text = str;


        }
        else {

            _Text_Hitokoto.text = "     パンダ";


        }



    }

   
    /// <summary>
    /// 白黒の比率状態
    /// </summary>
    public void Display_Image()
    {
        if (Black_Percent >= 9)
        {

            var image = _Result_Panda.GetComponent<Image>();
            image.sprite = _Result_Panda_sprites[2];

        }
        else if (Black_Percent >= 7)
        {

            var image = _Result_Panda.GetComponent<Image>();
            image.sprite = _Result_Panda_sprites[1];


        }
        else if (Black_Percent <= 3)
        {
            var image = _Result_Panda.GetComponent<Image>();
            image.sprite = _Result_Panda_sprites[3];


        }
        else if (Black_Percent <= 1)
        {

            var image = _Result_Panda.GetComponent<Image>();
            image.sprite = _Result_Panda_sprites[4];


        }
        else {

            var image = _Result_Panda.GetComponent<Image>();
            image.sprite = _Result_Panda_sprites[0];

        }




    }

    /// <summary>
    /// ポップアっプの表示
    /// </summary>
   public void Tap_Result_Button() {


        Rerult_Panel.gameObject.SetActive(true);



    }


    public void Tap_Cancel_Button()
    {


        Rerult_Panel.gameObject.SetActive(false);



    }










}
