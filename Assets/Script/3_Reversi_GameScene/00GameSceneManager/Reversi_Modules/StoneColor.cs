using UnityEngine;

/// <summary>
/// 石の色を管理するクラス
/// </summary>
public class StoneColor : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _Stone;

    // プロパティ
    public GridManager.estoneState StoneState
    {
        get => _stoneState;
        set
        {
            _stoneState = value;
            ChangeStoneColor();
        }
    }

    private GridManager.estoneState _stoneState;

    private void ChangeStoneColor()
    {
        switch (StoneState)
        {
            case GridManager.estoneState.EMPTY:
                _Stone.enabled = false;
                break;

            case GridManager.estoneState.CANTURN:
                _Stone.enabled = false;
                break;

            case GridManager.estoneState.BLACK:
                _Stone.enabled = true;
                _Stone.material.color = Color.black;
                break;

            case GridManager.estoneState.WHITE:
                _Stone.enabled = true;
                _Stone.material.color = Color.white;
                break;
        }
    }

    public void StoneColor_Reset()
    {
        if (StoneState == GridManager.estoneState.CANTURN) StoneState = GridManager.estoneState.EMPTY;
    }
}
