using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
/// <summary>
/// ゲームシーン制御クラス
/// プレイ開始前のオプション画面で下記情報をPlayerPrefで受け取り変数に代入する
/// ・先攻（白・黒）
/// ・ターン進行
/// ・CPUのAIレベル
/// </summary>


public class GameScene_Controller : MonoBehaviour
{
    public static GameScene_Controller Instance {

        get { return instance; }
    }

    static GameScene_Controller instance;

    [SerializeField]
    private GridManager _GridManeger;

    //ターン進行管理
    public GridManager.estoneState MyTurn;

    //AIのレベル管理
    public GridManager.eLevelState MyEnemy_LEVEL;

    //白・黒どちらの色を選択したか
    public GridManager.estoneState Choice_Stone_Color;

    //Choice_Stone_Colorで選択した色をMyTurnに代入するための変数
    public GridManager.estoneState Choiceng_Stone;

    [SerializeField]
    private UI_Managaer _UIManager;

    //石のカウント
    private int StoneCount;

    //クリアフラグ
    public bool Player_Win = false;
    public bool Player_Lose = false;
    public bool Player_Draw = false;


    //データ保存用変数
    private int _Select_Stone_Load;
    private int _CPU_Level_Load;


    void Awake() {

        instance = this;
        
    }

    void Start()
    {
        //AIレベル選定　playerPrefで所得
         _CPU_Level_Load =PlayerPrefs.GetInt(SaveData_Manager.KEY_CPU_LEVEL, 0);

        Instance.MyEnemy_LEVEL = (GridManager.eLevelState)_CPU_Level_Load;

        //先行か後攻用　playPrefで所得
        _Select_Stone_Load = PlayerPrefs.GetInt(SaveData_Manager.KEY_STONE_SELECT, 0);
        Instance.Choice_Stone_Color = (GridManager.estoneState)_Select_Stone_Load;

        //先行か後攻かの判定
        if (Instance.Choice_Stone_Color == GridManager.estoneState.BLACK) { Instance.MyTurn = GridManager.estoneState.BLACK; }
        if (Instance.Choice_Stone_Color == GridManager.estoneState.WHITE) { Instance.MyTurn = GridManager.estoneState.WHITE; }
      
        Instance.Choiceng_Stone = Instance.MyTurn;

        //PlayerPrefs.DeleteAll();


        //勝負判定を1回のみ行いたいのでUniRxを使用
        this.UpdateAsObservable()
        .Where(_ => Instance.Player_Win == true)
        .Take(1)
        .Subscribe(onNext_ => {


            Instance.Game_Player_Win();
            Debug.Log("UniRx通過");

        });

        this.UpdateAsObservable()
        .Where(_ => Instance.Player_Lose == true)
        .Take(1)
        .Subscribe(onNext_ => {


            Instance.Game_Player_Lose();
            Debug.Log("UniRx通過");

        });

        this.UpdateAsObservable()
        .Where(_ => Instance.Player_Draw == true)
        .Take(1)
        .Subscribe(onNext_ => {


            Instance.Game_Player_Lose();
            Debug.Log("UniRx通過");

        });


        //プレイヤーが白を選んだら1ターンスキップする
        if (Instance.MyTurn == GridManager.estoneState.WHITE)
        {
     
            Instance.MyTurn = 
                ((Instance.MyTurn == GridManager.estoneState.BLACK) ? 
                GridManager.estoneState.WHITE : GridManager.estoneState.BLACK);
         
        }

        //リバーシ盤・石の生成
        _GridManeger.Grid_Prefab_Module_Make();

        _UIManager._Select_UI_Stone_Set();

        _UIManager._Select_UI_Now();

       // _GridManeger.Play_End_Cheack(Instance.MyTurn);


    }

  

    void Update()
    {

        //ターン進行管理
        if (Instance.Choiceng_Stone == Instance.MyTurn)
        {
            //自分が選択した色のターン
            _GridManeger.Player_Now_Turn();

        }
        else
        {
            //CPUターン
            _GridManeger.Enemy_Now_Turn();

        }

        
     
    }

    //勝敗キーの書き込み・セーブ処理
    public void Game_Player_Win() {

       
            if (GameScene_Controller.Instance.Choice_Stone_Color == GridManager.estoneState.BLACK)
            {
                var blackwin = PlayerPrefs.GetInt("BLACKWIN", 0);

                blackwin++;
                PlayerPrefs.SetInt("BLACKWIN", blackwin);

            }
            else if (GameScene_Controller.Instance.Choice_Stone_Color == GridManager.estoneState.WHITE)
            {

                var whitewin = PlayerPrefs.GetInt("WHITEWIN", 0);
                whitewin++;
                PlayerPrefs.SetInt("WHITEWIN", whitewin);

            }

            var esaplus = PlayerPrefs.GetInt("ESA", 0);
            esaplus += 3;

            PlayerPrefs.SetInt("ESA", esaplus);


    }

    public void Game_Player_Lose()
    {
      
            var esaplus = PlayerPrefs.GetInt("ESA", 0);
            esaplus += 1;

            PlayerPrefs.SetInt("ESA", esaplus);


    }





}
