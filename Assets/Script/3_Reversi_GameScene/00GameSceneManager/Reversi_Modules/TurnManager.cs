using System.Collections.Generic;
using UnityEngine;
using static GridManager;

/// <summary>
/// 石のターンを管理するクラス
/// </summary>
public class TurnManager : MonoBehaviour
{
    //ひっくり返す座標をリストに入れる
    public class Turnstone_c
    {
        public int c_z;
        public int c_x;

        //ひっくり返したときのターンの保存
        public StoneState ENEMY_;

        //コンストラクタ・もし、探索した方向に敵の色があったら、このリストに座標位置を入れる
        public Turnstone_c(int z, int x)
        {
            c_z = z;
            c_x = x;

        }

        //コンストラクタ・敵のUndoリスト生成
        public Turnstone_c(int z, int x, StoneState ENEMY)
        {
            c_z = z;
            c_x = x;
            ENEMY_ = ENEMY;
        }
    }

    //タップした石からの8方向の確認をする
    private int[] Turn_CHECK_X = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
    private int[] Turn_CHECK_Z = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };

    /// <summary>
    /// ターン可能の座標位置に石とFieldの状態をCANTURNにする, stonecheckは勝敗チェックの際に使用し
    /// 両社ともCANTURNがあるかどうかを調べる。
    /// </summary>
    /// <param name="nowturn"></param>
    /// <param name="list"></param>
    /// <param name="fields"></param>
    /// <param name="stones"></param>
    public void TurnColorListGet(StoneState nowturn, List<Turnstone_c> list, Field[,] fields, StoneColor[,] stones, bool stonecheck)
    {
        StoneState enemyStone = ((nowturn == StoneState.Black) ? StoneState.White : StoneState.Black);

        for (var i = 0; i < Turn_CHECK_Z.Length; i++)
        {
            for (var k = 0; k < Turn_CHECK_X.Length; k++)
            {
                //石がTurnの色と一致したら
                if (stones[i, k].StoneState == nowturn)
                {
                    for (var p = 0; p < Turn_CHECK_X.Length; p++)
                    {
                        var z1 = i;
                        var x1 = k;

                        if (!stonecheck) list.Clear();

                        //MyTurn位置を中心に8方向（画面端）に検索をかける
                        while (true)
                        {
                            z1 += Turn_CHECK_Z[p];
                            x1 += Turn_CHECK_X[p];

                            var z_plus = z1 + Turn_CHECK_Z[p];
                            var x_plus = x1 + Turn_CHECK_X[p];

                            //現在の座標位置を+1方向した位置が盤外だったらbreak
                            if (!(x1 >= 0 && x1 < rows && z1 >= 0 && z1 < cols)) break;

                            //現在の座標位置を+1方向した位置の石の状態がMyTurnと同じ色であればbreak
                            if (stones[z1, x1].StoneState == nowturn) break;

                            //現在の座標位置を+2方向した位置が盤外だったらbreak
                            if (!(x_plus >= 0 && x_plus < rows && z_plus >= 0 && z_plus < cols)) break;

                            //現在の座標位置を+2方向した位置の石の状態がEMPTYだったら
                            if (stones[z1, x1].StoneState == enemyStone && stones[z_plus, x_plus].StoneState == StoneState.Empty)
                            {

                                //EMPTYの位置をリストに保存
                                list.Add(new Turnstone_c(z_plus, x_plus));

                                break;

                            }//+2方向した位置がMyTurnと逆の色であれば検索をつづける
                            else if (stones[z1, x1].StoneState == enemyStone) { continue; }
                            else { break; }
                        }

                        if (!stonecheck)
                        {
                            foreach (var canturn in list)
                            {
                                //石の状態はCANTURNにする
                                stones[canturn.c_z, canturn.c_x].StoneState = StoneState.CanTurn;
                                fields[canturn.c_z, canturn.c_x].GetFieldStone = FirldState.CanTurn;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// タップした座標から8方向チェックし、ターンできる石がある場合はリストに格納し、まとめてターンをする
    /// enemyundoがオンの場合は、2手先に戻す処理をするため、ひっくり返したすべての石の情報と、石を置いた位置を保存する
    /// </summary>
    /// <param name="nowturn"></param>
    /// <param name="list"></param>
    /// <param name="fields"></param>
    /// <param name="stones"></param>
    public void TurnCheak(List<Turnstone_c> turnlist, List<Turnstone_c> undolist, StoneState nowturn, int x, int y, StoneColor[,] stones, bool enemyundo)
    {
        bool CanTurn_;

        //MyTurnではない色
        StoneState enemyStone = ((nowturn == StoneState.Black) ? StoneState.White : StoneState.Black);

        for (var i = 0; i < Turn_CHECK_X.Length; i++)
        {
            //タップしたx・z情報をローカル変数に代入
            int stonex = x;
            int stonez = y;

            CanTurn_ = false;

            turnlist.Clear();

            //全8方向・1方向ずつ石の状態を確認する
            while (true)
            {
                stonez += Turn_CHECK_Z[i];
                stonex += Turn_CHECK_X[i];

                //座標がxが0以下で、8より大きかったらwhileからbreakする
                if (!(stonex >= 0 && stonex < rows && stonez >= 0 && stonez < cols)) break;

                //もし、enemystoneがあったらリストに格納する
                if (stones[stonez, stonex].StoneState == enemyStone)
                {
                    turnlist.Add(new Turnstone_c(stonez, stonex));

                }//MyTurnの色と一致する石であれば
                else if (stones[stonez, stonex].StoneState == nowturn)
                {

                    CanTurn_ = true;
                    break;

                }//もし、何も置かれていない石であればbreakする
                else if (stones[stonez, stonex].StoneState == StoneState.Empty)
                {
                    break;
                }
            }

            if (CanTurn_)
            {
                foreach (var canturn in turnlist)
                {
                    //リストに入った石をTurnの色に変更する
                    stones[canturn.c_z, canturn.c_x].StoneState = nowturn;

                    //敵AIリスト限定・元の手に戻せるようにひっくり返す座標を格納しておく
                    if (enemyundo) undolist.Add(new Turnstone_c(canturn.c_z, canturn.c_x, nowturn));
                }
            }
        }
    }
}
