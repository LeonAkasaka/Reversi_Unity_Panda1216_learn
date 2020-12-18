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
    private List<TurnStone> LEV3_TurnList = new List<TurnStone>();

    private List<TurnStone> LEV3_Undoist = new List<TurnStone>();

    /// <summary>
    /// LEVEL1:ターン可能リストからランダムで選択
    /// </summary>
    /// <param name="canputstone"></param>
    /// <returns></returns>
    public TurnStone LEVEL1_Return_Stone(List<TurnStone> canputstone)
    {
        var rerult = canputstone[Random.Range(0, canputstone.Count)];
        return rerult;
    }

    /// <summary>
    /// LEVEL2:ターン可能リストから最も強い評価値を選択
    /// </summary>
    /// <param name="canputstone"></param>
    /// <returns></returns>
    public TurnStone LEVEL2_Return_Stone(List<TurnStone> canputstone)
    {
        TurnStone rerult = canputstone[0];
        var score = ScoreCount.Eva_Score[rerult.Z, rerult.X];

        foreach (var i in canputstone)
        {

            if (score <= ScoreCount.Eva_Score[i.Z, i.X])
            {
                score = ScoreCount.Eva_Score[i.Z, i.X];
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
    public TurnStone LEVEL3_Return_Stone(List<TurnStone> canputstone, StoneColor[,] stones, Field[,] field, StoneState nowturn)
    {
        //反対の石
        StoneState enemyStone = ((nowturn == StoneState.Black) ? StoneState.White : StoneState.Black);

        TurnStone rerult = canputstone[0];

        //最も高い評価値を保存
        var resultScore = 0;

        //1手目
        foreach (var i in canputstone)
        {
            //リストのクリア
            LEV3_TurnList.Clear();
            LEV3_Undoist.Clear();

            //i番目の座標をターン
            stones[i.Z, i.X].StoneState = nowturn;

            //ターンチェック
            bool can = true;
            _TurnManager.TurnCheak(LEV3_TurnList, LEV3_Undoist, nowturn, i.X, i.Z, stones, can);

            Debug.Log("i_x=" + i.X + "i_y=" + i.Z);

            //ターンした後に、
            bool cannot = false;
            _TurnManager.TurnColorListGet(enemyStone, LEV3_TurnList, field, stones, cannot);

            Enemy_Stone_Select(LEV3_TurnList, stones);

            //2手目
            foreach (var k in LEV3_TurnList)
            {
                //スコアを採点する（第一手目をプラスしていく）
                var score = ScoreCount.Return_Eve_Num(stones, nowturn);
                var testscore = ScoreCount.Eva_Score[i.Z, i.X];
                score += testscore;

                //最も高得点の評価値を選びscoreにマイナスしたものをscoreに入れる
                var stackscore = (-1 * ScoreCount.Eva_Score[k.Z, k.X]);
                // Debug.Log("マイナス　" + stackscore );

                score += stackscore;

                Debug.Log("k_x=" + k.X + "k_y=" + k.Z + "  Score" + score);

                //現在のスコアがrerultscoreよりも大きかったら
                if (resultScore <= score)
                {

                    resultScore = score;

                    Debug.Log("リザルト：" + resultScore);
                    Debug.Log("代入されました" + "x=" + k.X + "y=" + k.Z + "  Score" + score);

                    //returnするクラスを更新する
                    rerult = i;
                }
            }

            var u = new TurnStone(i.Z, i.X);
            Backstone(nowturn, LEV3_Undoist, stones, u, field);
        }

        Debug.Log("最終リザルト" + "x=" + rerult.X + "y=" + rerult.Z + "Score" + resultScore);
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
    public void Backstone(StoneState nowturn, List<TurnStone> undolist, StoneColor[,] stones, TurnStone undoxy, Field[,] field)
    {
        //反対のターン情報を変数に代入する
        StoneState enemyStone = ((nowturn == StoneState.Black) ? StoneState.White : StoneState.Black);

        //ひっくり返す前のターンに戻す
        foreach (var i in undolist)
        {
            stones[i.Z, i.X].StoneState = enemyStone;
        }

        //undoリストの情報から前の手に戻す
        stones[undoxy.Z, undoxy.X].StoneState = StoneState.CanTurn;

        Enemy_Reset_Color(stones);

        Enemy_Reset_Field(field);
    }

    /// <summary>
    /// CPU専用　CANTURN状態の石を2手先読むためのリストに入れる
    /// </summary>
    /// <param name="CanTurnList"></param>
    /// <param name="stones"></param>
    public void Enemy_Stone_Select(List<TurnStone> CanTurnList, StoneColor[,] stones)
    {
        CanTurnList.Clear();

        for (var i = 0; i < GridManager.cols; i++)
        {
            for (var k = 0; k < GridManager.rows; k++)
            {
                if (stones[i, k].StoneState == StoneState.CanTurn) CanTurnList.Add(new TurnStone(i, k));
            }
        }
    }

    public void Enemy_Stone_Select_Check(List<TurnStone> CanTurnList, StoneColor[,] stones)
    {
        for (var i = 0; i < GridManager.cols; i++)
        {
            for (var k = 0; k < GridManager.rows; k++)
            {
                if (stones[i, k].StoneState == StoneState.CanTurn) CanTurnList.Add(new TurnStone(i, k));
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
