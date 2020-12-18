using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石の色の状態管理
/// </summary>
public enum StoneState
{
    Empty,
    Black,
    White,
    CanTurn,
}

/// <summary>
/// オセロ盤のターン可能位置を表示用の状態管理
/// </summary>
public enum FirldState
{
    CanTurn,
    NotTurn,
}

/// <summary>
/// CPUのレベル（playerprefで所得）
/// </summary>
public enum LevelState
{ //難易度1～3

    Level1,
    Level2,
    Level3,
}

/// <summary>
/// オセロ盤の管理クラス
/// </summary>
public class GridManager : MonoBehaviour
{
    /// <summary>
    /// 石の管理用
    /// </summary>
    [SerializeField]
    private GameObject _StonePrefab;

    [SerializeField]
    private GameObject _Canvas;

    private StoneColor[,] _StoneManager = new StoneColor[Cols, Rows];

    //8*8生成用
    public static int Cols { get; } = 8;
    public static int Rows { get; } = 8;


    /// <summary>
    /// オセロ盤のターン可能位置を表示用変数
    /// </summary>
    [SerializeField]
    private GameObject _FieldPrefab;

    private Field[,] _FieldManager = new Field[Cols, Rows];

    /// <summary>
    /// 石のターン用
    /// </summary>
    [SerializeField]
    private TurnManager _TurnManager;

    //タップ座標格納用変数
    private int x;
    private int z;

    //探索時にターンできる石を格納する
    private List<TurnStone> _TurnList = new List<TurnStone>();

    //ターン可能な石を格納する
    private List<TurnStone> _TurnColorList = new List<TurnStone>();

    //CPU　AI用　石の状態を1手先戻すために必要な座標情報を格納する
    private List<TurnStone> _UndoList = new List<TurnStone>();


    List<TurnStone> player_puttingcancheck = new List<TurnStone>();
    List<TurnStone> CPU_puttingcancheck = new List<TurnStone>();


    //ターン開始際の石のターンする際の演出のための非同期処理管理
    //コルーチンの唯一性を保つためのフラグ管理　※今後は、全ターン管理をコルーチン処理をしていきたい
    private bool CoroutineWaiting = false;

    private bool EnemyColutinWaiting = false;

    private bool SkipColutinWaiting = false;


    /// <summary>
    /// UI管理用
    /// </summary>
    [SerializeField]
    private UI_Managaer _UI_Managaer;

    public bool all_stone_player_same;

    public bool all_stone_CPU_same;

    public bool all_stone_Draw_same;

    /// <summary>
    /// CPU AIの管理用
    /// </summary>
    [SerializeField]
    private Enemy_AI _Enemy_AI;

    /// <summary>
    /// 評価値管理用
    /// </summary>
    [SerializeField]
    private Evaluation_Score _Evaluation_Score_Count;

    private int Player_Stone_Count;
    private int CPU_Stone_Count;

    /// <summary>
    /// 石とターン可能位置の生成
    /// </summary>
    public void Grid_Prefab_Module_Make()
    {
        for (var i = 0; i < Cols; i++)
        {
            for (var k = 0; k < Rows; k++)
            {
                //石の生成
                var stone = Instantiate(_StonePrefab);

                stone.transform.parent = _Canvas.transform;

                //ボードの大きさと石の大きさをあわせるために微調整
                stone.transform.localPosition = new Vector3(k + (k * 0.3f), 0, i + (i * 0.2f));
                stone.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                stone.transform.Rotate(new Vector3(-90, 0, 0));

                stone.name = "Stone" + i + "  " + k;

                _StoneManager[i, k] = stone.GetComponent<StoneColor>();
                _StoneManager[i, k].StoneState = StoneState.Empty;

                //ターン可能位置の配置
                var field = Instantiate(_FieldPrefab, new Vector3(k, i, 20), Quaternion.Euler(0, 0, 0));

                field.transform.parent = _Canvas.transform;

                field.transform.localPosition = new Vector3(k + (k * 0.3f), 0, i + (i * 0.2f));
                field.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                field.transform.Rotate(new Vector3(-90, 0, 0));

                field.name = "field" + i + "  " + k;

                _FieldManager[i, k] = field.GetComponent<Field>();
                _FieldManager[i, k].GetFieldStone = FirldState.NotTurn;
            }
        }

        //初期配置
        _StoneManager[4, 3].StoneState = StoneState.White;
        _StoneManager[4, 4].StoneState = StoneState.Black;
        _StoneManager[3, 3].StoneState = StoneState.Black;
        _StoneManager[3, 4].StoneState = StoneState.White;
    }

    /// <summary>
    /// プレイヤーターン進行用
    /// </summary>
    public void Player_Now_Turn()
    {
        //コルーチンの処理の際は停止をさせる※不具合原因になりやすいフラグにつき改善していきたい
        if (!CoroutineWaiting)
        {
            //石を置ける状態を検索する
            bool cannot = false;
            _TurnManager.TurnColorListGet(GameScene_Controller.Instance.MyTurn, _TurnColorList, _FieldManager, _StoneManager, cannot);

            //TurnColorListに入れる
            _Enemy_AI.Enemy_Stone_Select(_TurnColorList, _StoneManager);

            //TurnColorListはNULL
            if (!(_TurnColorList != null && _TurnColorList.Count > 0))
            {
                if (!all_stone_player_same && !all_stone_CPU_same) StartCoroutine(SkipCoroutin());
            }
        }

        //デバッグモード用
        if (Input.GetKey(KeyCode.A))
        {
            funcWHITE();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var x_w = hit.collider.gameObject.transform.localPosition.x;
                var z_w = hit.collider.gameObject.transform.localPosition.z;

                //座標補正
                if (x_w <= 1) { x_w = 0; }
                else { x_w -= x_w * 0.2f; }

                z_w -= z_w * 0.1f;

                x = (int)x_w;
                z = (int)z_w;

                //タップした座標の石の状態が置ける状態であれば
                if (_StoneManager[z, x].StoneState == StoneState.CanTurn)
                {
                    //盤内であれば
                    if (x >= 0 && x < Rows && z >= 0 && z < Cols)
                    {
                        //タップ座標の石を置く
                        _StoneManager[z, x].StoneState = GameScene_Controller.Instance.MyTurn;

                        //石のターン処理を開始する
                        StartCoroutine(PlayerTurnCoroutin());
                    }
                    else
                    {
                        //置ける石の場所がなければスキップする
                        if (!SkipColutinWaiting)
                        {

                            if (!all_stone_player_same && !all_stone_CPU_same && !all_stone_Draw_same) StartCoroutine(SkipCoroutin());
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// プレイヤーのターン処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerTurnCoroutin()
    {
        //非同期処理の間、唯一性を保つための処理
        if (CoroutineWaiting) yield break;

        CoroutineWaiting = true;

        Reset_Color();

        SoundManager.Instance.SE_Stone_Tap_Play();
        yield return new WaitForSeconds(0.3f);

        //石のターンチェックをする
        bool cannot = false;
        _TurnManager.TurnCheak(_TurnList, _UndoList, GameScene_Controller.Instance.MyTurn, x, z, _StoneManager, cannot);
        SoundManager.Instance.SE_Stone_Turn();
        yield return new WaitForSeconds(0.3f);

        Play_End_Cheack(GameScene_Controller.Instance.MyTurn);

        yield return new WaitForSeconds(0.3f);

        //CPUのターンへ
        GameScene_Controller.Instance.MyTurn =
            ((GameScene_Controller.Instance.MyTurn == StoneState.Black) ? StoneState.White : StoneState.Black);

        _UI_Managaer._Select_UI_Now();

        CoroutineWaiting = false;
    }

    /// <summary>
    /// CPUtターン進行用
    /// </summary>
    public void Enemy_Now_Turn()
    {
        if (!EnemyColutinWaiting)
        {
            bool cannot = false;
            _TurnManager.TurnColorListGet(GameScene_Controller.Instance.MyTurn, _TurnList, _FieldManager, _StoneManager, cannot);
            _Enemy_AI.Enemy_Stone_Select(_TurnColorList, _StoneManager);

            //NULLチェック            
            if (_TurnColorList != null && _TurnColorList.Count > 0)
            {
                //選択したCPUのレベルによって置く石の座標を変える
                var rerult = Enemy_AI_Level_Get(GameScene_Controller.Instance.MyEnemy_LEVEL);

                x = rerult.X;
                z = rerult.Z;

                //石のターン処理を開始する
                StartCoroutine(EnemyTurnCoroutin());
            }
            else
            {
                if (!SkipColutinWaiting)
                {
                    if (!all_stone_player_same && !all_stone_CPU_same && !all_stone_Draw_same) StartCoroutine(SkipCoroutin());
                }
            }
        }
    }

    /// <summary>
    /// CPUのターン処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyTurnCoroutin()
    {
        if (EnemyColutinWaiting) yield break;

        EnemyColutinWaiting = true;

        Reset_Color();

        yield return new WaitForSeconds(1.0f);
        SoundManager.Instance.SE_Stone_Tap_Play();
        _StoneManager[z, x].StoneState = GameScene_Controller.Instance.MyTurn;

        yield return new WaitForSeconds(0.2f);

        //石のターンチェックをする
        bool cannot = false;
        _TurnManager.TurnCheak(_TurnList, _UndoList, GameScene_Controller.Instance.MyTurn, x, z, _StoneManager, cannot);
        SoundManager.Instance.SE_Stone_Turn();

        yield return new WaitForSeconds(0.3f);

        Play_End_Cheack(GameScene_Controller.Instance.MyTurn);

        yield return new WaitForSeconds(0.3f);

        //プレイヤーのターンへ
        GameScene_Controller.Instance.MyTurn =
            ((GameScene_Controller.Instance.MyTurn == StoneState.Black) ? StoneState.White : StoneState.Black);

        _UI_Managaer._Select_UI_Now();
        EnemyColutinWaiting = false;
    }

    /// <summary>
    /// スキップ処理のコルーチン
    /// </summary>
    private IEnumerator SkipCoroutin()
    {
        if (SkipColutinWaiting) yield break;

        SkipColutinWaiting = true;
        _UI_Managaer.SkipON();
        yield return new WaitForSeconds(1.0f);

        GameScene_Controller.Instance.MyTurn =
           ((GameScene_Controller.Instance.MyTurn == StoneState.Black) ? StoneState.White : StoneState.Black);
        _UI_Managaer.SkipOFF();

        SkipColutinWaiting = false;
    }

    /// <summary>
    /// 選択したCPUのAIレベルに応じた座標配置を行う
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public TurnStone Enemy_AI_Level_Get(LevelState level)
    {
        switch (level)
        {
            case LevelState.Level1:
                {
                    var result = _Enemy_AI.LEVEL1_Return_Stone(_TurnColorList);
                    Debug.Log("レベル1");
                    return result;
                }
            case LevelState.Level2:
                {
                    var result = _Enemy_AI.LEVEL2_Return_Stone(_TurnColorList);
                    Debug.Log("レベル2");
                    return result;
                }
            case LevelState.Level3:
                {
                    var result = _Enemy_AI.LEVEL3_Return_Stone(_TurnColorList, _StoneManager, _FieldManager, GameScene_Controller.Instance.MyTurn);
                    Debug.Log("レベル3");
                    return result;
                }
            default:
                {
                    var result = _Enemy_AI.LEVEL1_Return_Stone(_TurnColorList);
                    Debug.Log("レベルが未選択エラー");
                    return result;
                }
        }
    }

    /// <summary>
    /// （MyTurn）ターン遷移時に石・フィールドの状態をリセットする
    /// </summary>
    public void Reset_Color()
    {
        for (var i = 0; i < Cols; i++)
        {
            //石の配置
            for (var k = 0; k < Rows; k++)
            {
                _StoneManager[i, k].StoneColor_Reset();
                _FieldManager[i, k].GetFieldStone = FirldState.NotTurn;
            }
        }
    }

    /// <summary>
    /// 試合終了フラグの確認・現在置かれている石のチェック（64個置かれていたら終了）
    /// 置かれてる石がすべて同じ色だったら終了チェック。
    /// </summary>
    public void Play_End_Cheack(StoneState nowturn)
    {
        //自分が選んでいない石のターン
        StoneState notchoisestone = ((GameScene_Controller.Instance.Choiceng_Stone == StoneState.Black) ? StoneState.White : StoneState.Black);

        //Playre
        Player_Stone_Count = _Evaluation_Score_Count.StoneCount(_StoneManager, GameScene_Controller.Instance.Choiceng_Stone);

        CPU_Stone_Count = _Evaluation_Score_Count.StoneCount(_StoneManager, notchoisestone);

        all_stone_player_same = _Evaluation_Score_Count.All_Stone_Color_Count_Check(_StoneManager, GameScene_Controller.Instance.Choiceng_Stone);

        all_stone_CPU_same = _Evaluation_Score_Count.All_Stone_Color_Count_Check(_StoneManager, notchoisestone);

        var score = Player_Stone_Count + CPU_Stone_Count;

        if (score >= Cols * Rows)
        {
            Debug.Log("試合終了フラグ");

            Win_Lose_Rerult();
        }
        else
        {

            Debug.Log("PLAYER" + Player_Stone_Count + "ENEMY" + CPU_Stone_Count);

        }

        IsPutting_Stone_Check();
    }

    /// <summary>
    /// 64個以下だが、置ける場所がない場合ゲームの終了処理を行う
    /// </summary>
    public void IsPutting_Stone_Check()
    {
        //選んでない色の石のターン
        StoneState notchoisestone = ((GameScene_Controller.Instance.Choiceng_Stone == StoneState.Black) ? StoneState.White : StoneState.Black);

        //PlayerとCPU両方で置くことができる石の状態を検索・検索したplayer_puttingcancheckをに入れる
        bool can = true;
        _TurnManager.TurnColorListGet(GameScene_Controller.Instance.Choiceng_Stone, player_puttingcancheck, _FieldManager, _StoneManager, can);

        _TurnManager.TurnColorListGet(notchoisestone, player_puttingcancheck, _FieldManager, _StoneManager, can);

        _Enemy_AI.Enemy_Stone_Select_Check(player_puttingcancheck, _StoneManager);

        //リストになにもはいっていなかったら勝敗判定を出す
        if (!(player_puttingcancheck != null && player_puttingcancheck.Count > 0))
        {
            Win_Lose_Rerult();
        }
        else
        {
            //リストに入っていたら、リストをクリアする
            player_puttingcancheck.Clear();
            CPU_puttingcancheck.Clear();
        }
    }

    /// <summary>
    /// 勝敗判定
    /// </summary>
    public void Win_Lose_Rerult()
    {
        Debug.Log("試合終了フラグ");

        if (Player_Stone_Count > CPU_Stone_Count)
        {
            Debug.Log("勝ち");
            all_stone_player_same = true;
            GameScene_Controller.Instance.Player_Win = true;
            _UI_Managaer.Player_Win();
        }
        else if (Player_Stone_Count < CPU_Stone_Count)
        {
            Debug.Log("負け");
            all_stone_CPU_same = true;
            GameScene_Controller.Instance.Player_Lose = true;
            _UI_Managaer.Player_Lose();
        }
        else
        {
            Debug.Log("ドロー");
            all_stone_Draw_same = true;
            GameScene_Controller.Instance.Player_Draw = true;
            _UI_Managaer.Player_Draw();
        }
    }

    /// <summary>
    /// デバッグメニュー、全部黒にする
    /// </summary>
    public void funcWHITE()
    {
        for (var i = 0; i < Cols; i++)
        {
            //石の配置
            for (var k = 0; k < Rows; k++)
            {
                if (k % 2 == 0)
                {
                    _StoneManager[i, k].StoneState = StoneState.Black;

                }
                else
                {
                    _StoneManager[i, k].StoneState = StoneState.White;
                }
                //_StoneManager[i, k].StoneState = estoneState.BLACK;
            }
        }

        Play_End_Cheack(GameScene_Controller.Instance.MyTurn);
    }
}
