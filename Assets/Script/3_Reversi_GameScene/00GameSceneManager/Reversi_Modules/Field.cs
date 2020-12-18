using UnityEngine;

/// <summary>
/// オセロ盤のターン可能位置を表示するクラス
/// </summary>
public class Field : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer CanFirld;

    public FirldState GetFieldStone
    {
        get => FirldState;
        set
        {
            FirldState = value;
            Field_CanTurnColor();
        }
    }

    private FirldState FirldState;

    /// <summary>
    /// オセロ盤のターン可能位置の状態を管理。CANTURNで赤いブロックが表示される。
    /// </summary>
    public void Field_CanTurnColor()
    {
        switch (FirldState)
        {
            case FirldState.NotTurn:
                CanFirld.enabled = false;
                break;
            case FirldState.CanTurn:
                CanFirld.enabled = true;
                CanFirld.material.color = Color.red;
                break;
        }
    }

    /// <summary>
    /// ターン可能位置の状態をNOTTURNとし、赤いブロックを非表示にして次ターンへいく
    /// </summary>
    public void Field_Reset()
    {
        if (FirldState == FirldState.CanTurn)
        {
            FirldState = FirldState.NotTurn;
        }
    }
}
