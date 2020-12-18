using System.Collections.Generic;
using UnityEngine;
using static GridManager;
using static TurnManager;

/// <summary>
/// CPUのAIを管理するクラス
/// </summary>
public class Enemy_AI : MonoBehaviour
{
    [SerializeField]
    private Evaluation_Score ScoreCount;

    [SerializeField]
    private TurnManager _TurnManager;

    [SerializeField]
    private Field _FieldManager;

    //LEVEL3用（2手先読む）のターンリスト格納用
    private List<Turnstone_c> LEV3_TurnList = new List<Turnstone_c>();

    private List<Turnstone_c> LEV3_Undoist = new List<Turnstone_c>();

    /// <summary>
    /// LEVEL1:ターン可能リストからランダムで選択
    /// </summary>
    /// <param name="canputstone"></param>
    /// <returns></returns>
    public Turnstone_c LEVEL1_Return_Stone(List<Turnstone_c> canputstone)
    {
        var rerult = canputstone[Random.Range(0, canputstone.Count)];
        return rerult;
    }

    /// <summary>
    /// LEVEL2:ターン可能リストから最も強い評価値を選択
    /// </summary>
    /// <param name="canputstone"></param>
    /// <returns></returns>
    public Turnstone_c LEVEL2_Return_Stone(List<Turnstone_c> canputstone)
    {
        Turnstone_c rerult = canputstone[0];
        var score = ScoreCount.Eva_Score[rerult.c_z, rerult.c_x];

        foreach (var i in canputstone)
        {

            if (score <= ScoreCount.Eva_Score[i.c_z, i.c_x])
            {
                score = ScoreCount.Eva_Score[i.c_z, i.c_x];
                rerult = i;
            }
        }

        return rerult;
    }

    /// <summary>
    /// LEVEL3:ミニマックス法を使用し、2手先まで読む。
    /// （1手先：最も高い評価値　2手先：最も低い評価値を足した合計値が高い手を選択）
    /// </summary>
    /// <param name="canputstone"></param>
    /// <param name="stones"></param>
    /// <param name="nowturn"></param>
    /// <returns></returns>
    public Turnstone_c LEVEL3_Return_Stone(List<Turnstone_c> canputstone, StoneColor[,] stones, Field[,] field, estoneState nowturn)
    {
        //反対の石
        estoneState enemyStone = ((nowturn == estoneState.BLACK) ? estoneState.WHITE : estoneState.BLACK);

        Turnstone_c rerult = canputstone[0];

        //最も高い評価値を保存
        var resultScore = 0;

        //1手目
        foreach (var i in canputstone)
        {
            //リストのクリア
            LEV3_TurnList.Clear();
            LEV3_Undoist.Clear();

            //i番目の座標をターン
            stones[i.c_z, i.c_x].StoneState = nowturn;

            //ターンチェック
            bool can = true;
            _TurnManager.TurnCheak(LEV3_TurnList, LEV3_Undoist, nowturn, i.c_x, i.c_z, stones, can);

            Debug.Log("i_x=" + i.c_x + "i_y=" + i.c_z);

            //ターンした後に、
            bool cannot = false;
            _TurnManager.TurnColorListGet(enemyStone, LEV3_TurnList, field, stones, cannot);

            Enemy_Stone_Select(LEV3_TurnList, stones);

            //2手目
            foreach (var k in LEV3_TurnList)
            {
                //スコアを採点する（第一手目をプラスしていく）
                var score = ScoreCount.Return_Eve_Num(stones, nowturn);
                var testscore = ScoreCount.Eva_Score[i.c_z, i.c_x];
                score += testscore;

                //最も高得点の評価値を選びscoreにマイナスしたものをscoreに入れる
                var stackscore = (-1 * ScoreCount.Eva_Score[k.c_z, k.c_x]);
                // Debug.Log("マイナス　" + stackscore );

                score += stackscore;

                Debug.Log("k_x=" + k.c_x + "k_y=" + k.c_z + "  Score" + score);

                //現在のスコアがrerultscoreよりも大きかったら
                if (resultScore <= score)
                {

                    resultScore = score;

                    Debug.Log("リザルト：" + resultScore);
                    Debug.Log("代入されました" + "x=" + k.c_x + "y=" + k.c_z + "  Score" + score);

                    //returnするクラスを更新する
                    rerult = i;
                }
            }

            var u = new Turnstone_c(i.c_z, i.c_x, enemyStone);
            Backstone(nowturn, LEV3_Undoist, stones, u, field);
        }

        Debug.Log("最終リザルト" + "x=" + rerult.c_x + "y=" + rerult.c_z + "Score" + resultScore);
        return rerult;
    }

    /// <summary>
    /// 1手前の盤面に戻す
    /// </summary>
    /// <param name="nowturn"></param>
    /// <param name="undolist"></param>
    /// <param name="stones"></param>
    /// <param name="undoxy"></param>
    /// <param name="field"></param>
    public void Backstone(GridManager.estoneState nowturn, List<Turnstone_c> undolist, StoneColor[,] stones, Turnstone_c undoxy, Field[,] field)
    {
        //反対のターン情報を変数に代入する
        GridManager.estoneState enemyStone = ((nowturn == GridManager.estoneState.BLACK) ? GridManager.estoneState.WHITE : GridManager.estoneState.BLACK);

        //ひっくり返す前のターンに戻す
        foreach (var i in undolist)
        {
            stones[i.c_z, i.c_x].StoneState = enemyStone;
        }

        //undoリストの情報から前の手に戻す
        stones[undoxy.c_z, undoxy.c_x].StoneState = GridManager.estoneState.CANTURN;

        Enemy_Reset_Color(stones);

        Enemy_Reset_Field(field);
    }

    /// <summary>
    /// CPU専用　CANTURN状態の石を2手先読むためのリストに入れる
    /// </summary>
    /// <param name="CanTurnList"></param>
    /// <param name="stones"></param>
    public void Enemy_Stone_Select(List<Turnstone_c> CanTurnList, StoneColor[,] stones)
    {
        CanTurnList.Clear();

        for (var i = 0; i < GridManager.cols; i++)
        {
            for (var k = 0; k < GridManager.rows; k++)
            {
                if (stones[i, k].StoneState == GridManager.estoneState.CANTURN) CanTurnList.Add(new Turnstone_c(i, k));
            }
        }
    }

    public void Enemy_Stone_Select_Check(List<Turnstone_c> CanTurnList, StoneColor[,] stones)
    {
        for (var i = 0; i < GridManager.cols; i++)
        {
            for (var k = 0; k < GridManager.rows; k++)
            {
                if (stones[i, k].StoneState == GridManager.estoneState.CANTURN) CanTurnList.Add(new Turnstone_c(i, k));
            }
        }
    }

    /// <summary>
    /// 石の状態をリセット
    /// </summary>
    /// <param name="stones"></param>
    public void Enemy_Reset_Color(StoneColor[,] stones)
    {
        for (var i = 0; i < GridManager.cols; i++)
        {
            for (var k = 0; k < GridManager.rows; k++) { stones[i, k].StoneColor_Reset(); }
        }
    }

    /// <summary>
    ///  オセロ盤のターン可能位置をリセット
    /// </summary>
    /// <param name="stones"></param>
    public void Enemy_Reset_Field(Field[,] field)
    {
        for (var i = 0; i < GridManager.cols; i++)
        {
            for (var k = 0; k < GridManager.rows; k++) { field[i, k].Field_Reset(); }
        }
    }
}
