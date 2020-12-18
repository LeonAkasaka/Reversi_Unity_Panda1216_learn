using UnityEngine;

/// <summary>
/// 石の色を管理するクラス
/// </summary>
public class StoneColor : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _Stone;

    // プロパティ
    public StoneState StoneState
    {
        get => _stoneState;
        set
        {
            _stoneState = value;
            ChangeStoneColor();
        }
    }
    private StoneState _stoneState;

    private void ChangeStoneColor()
    {
        switch (StoneState)
        {
            case StoneState.Empty:
                _Stone.enabled = false;
                break;

            case StoneState.CanTurn:
                _Stone.enabled = false;
                break;

            case StoneState.Black:
                _Stone.enabled = true;
                _Stone.material.color = Color.black;
                break;

            case StoneState.White:
                _Stone.enabled = true;
                _Stone.material.color = Color.white;
                break;
        }
    }

    public void StoneColor_Reset()
    {
        if (StoneState == StoneState.CanTurn) StoneState = StoneState.Empty;
    }
}
