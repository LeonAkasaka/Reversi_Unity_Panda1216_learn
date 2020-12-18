using UnityEngine;

/// <summary>
/// 石の色を管理するクラス
/// </summary>
public class StoneColor : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _Stone;

    // プロパティ
    public estoneState StoneState
    {
        get => _stoneState;
        set
        {
            _stoneState = value;
            ChangeStoneColor();
        }
    }

    private estoneState _stoneState;

    private void ChangeStoneColor()
    {
        switch (StoneState)
        {
            case estoneState.EMPTY:
                _Stone.enabled = false;
                break;

            case estoneState.CANTURN:
                _Stone.enabled = false;
                break;

            case estoneState.BLACK:
                _Stone.enabled = true;
                _Stone.material.color = Color.black;
                break;

            case estoneState.WHITE:
                _Stone.enabled = true;
                _Stone.material.color = Color.white;
                break;
        }
    }

    public void StoneColor_Reset()
    {
        if (StoneState == estoneState.CANTURN) StoneState = estoneState.EMPTY;
    }
}
