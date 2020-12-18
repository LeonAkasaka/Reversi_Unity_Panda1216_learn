using UniRx;
using UniRx.Triggers;
using UnityEngine;

/// <summary>
/// ゲームシーン制御クラス
/// プレイ開始前のオプション画面で下記情報をPlayerPrefで受け取り変数に代入する
/// ・先攻（白・黒）
/// ・ターン進行
/// ・CPUのAIレベル
/// </summary>
public class GameScene_Controller : MonoBehaviour
{
    public static GameScene_Controller Instance { get; private set; }

    [SerializeField]
    private GridManager _GridManeger;

    //ターン進行管理
    public StoneState MyTurn;

    //AIのレベル管理
    public LevelState MyEnemy_LEVEL;

    //白・黒どちらの色を選択したか
    public StoneState Choice_Stone_Color;

    //Choice_Stone_Colorで選択した色をMyTurnに代入するための変数
    public StoneState Choiceng_Stone;

    [SerializeField]
    private UI_Managaer _UIManager;

    //クリアフラグ
    public bool Player_Win = false;
    public bool Player_Lose = false;
    public bool Player_Draw = false;

    //データ保存用変数
    private int _Select_Stone_Load;
    private int _CPU_Level_Load;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //AIレベル選定　playerPrefで所得
        _CPU_Level_Load = PlayerPrefs.GetInt(SaveData_Manager.KEY_CPU_LEVEL, 0);

        MyEnemy_LEVEL = (LevelState)_CPU_Level_Load;

        //先行か後攻用　playPrefで所得
        _Select_Stone_Load = PlayerPrefs.GetInt(SaveData_Manager.KEY_STONE_SELECT, 0);
        Choice_Stone_Color = (StoneState)_Select_Stone_Load;

        //先行か後攻かの判定
        if (Choice_Stone_Color == StoneState.Black) { MyTurn = StoneState.Black; }
        if (Choice_Stone_Color == StoneState.White) { MyTurn = StoneState.White; }

        Choiceng_Stone = MyTurn;

        //PlayerPrefs.DeleteAll();


        //勝負判定を1回のみ行いたいのでUniRxを使用
        this.UpdateAsObservable()
        .Where(_ => Player_Win == true)
        .Take(1)
        .Subscribe(onNext_ =>
        {
            Game_Player_Win();
            Debug.Log("UniRx通過");
        });

        this.UpdateAsObservable()
        .Where(_ => Player_Lose == true)
        .Take(1)
        .Subscribe(onNext_ =>
        {
            Game_Player_Lose();
            Debug.Log("UniRx通過");
        });

        this.UpdateAsObservable()
        .Where(_ => Player_Draw == true)
        .Take(1)
        .Subscribe(onNext_ =>
        {
            Game_Player_Lose();
            Debug.Log("UniRx通過");
        });

        //プレイヤーが白を選んだら1ターンスキップする
        if (MyTurn == StoneState.White)
        {
            MyTurn =
                ((MyTurn == StoneState.Black) ?
                StoneState.White : StoneState.Black);
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
        if (Choiceng_Stone == MyTurn)
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
    public void Game_Player_Win()
    {
        if (Choice_Stone_Color == StoneState.Black)
        {
            var blackwin = PlayerPrefs.GetInt("BLACKWIN", 0);

            blackwin++;
            PlayerPrefs.SetInt("BLACKWIN", blackwin);

        }
        else if (Choice_Stone_Color == StoneState.White)
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
