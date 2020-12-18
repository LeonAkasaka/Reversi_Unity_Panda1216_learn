using UnityEngine;

/// <summary>
/// オセロ盤のターン可能位置を表示するクラス
/// </summary>
public class Field : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer CanFirld;

    public efirldState GetFieldStone
    {
        get => FirldState;
        set
        {
            FirldState = value;
            Field_CanTurnColor();
        }
    }

    private efirldState FirldState;

    /// <summary>
    /// オセロ盤のターン可能位置の状態を管理。CANTURNで赤いブロックが表示される。
    /// </summary>
    public void Field_CanTurnColor()
    {
        switch (FirldState)
        {
            case efirldState.NOTTURN:
                CanFirld.enabled = false;
                break;
            case efirldState.CANTURN:
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
        if (FirldState == efirldState.CANTURN)
        {
            FirldState = efirldState.NOTTURN;
        }
    }
}
