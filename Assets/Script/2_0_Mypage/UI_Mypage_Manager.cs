using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// クラスの名称がややこしいので「Mypage_Play_popup」に変更予定　1216時点
/// マイページのPlayボタンを押したあとのポップアップ画面の管理クラス
/// 1216：まだアニメーションが入っていないのでDoTweenアニメーションを入れる予定
/// </summary>
public class UI_Mypage_Manager : MonoBehaviour
{
    /// <summary>
    /// CPUのレベル選択
    /// </summary>
    public enum eLevel_Select
    {
        uLevel1,
        uLevel2,
        uLevel3,
    }

    /// <summary>
    /// 何色を選んだかの選択
    /// </summary>
    public enum eSerect_Stone
    {
        uNONE,
        uBLACK,
        uWHITE,
    }

    [SerializeField]
    private Dropdown _CPU_Level_Dropdown;

    [SerializeField]
    private GameObject _GamePlay_PopupPanel;

    [SerializeField]
    private Image _Serect_Stone_Mark;

    [SerializeField]
    private GameObject[] _Panel_Brack_Or_White_Select = new GameObject[2];

    /// <summary>
    /// 黒を選んだらPlayerPrefsに黒をセット
    /// </summary>
    public void Select_Black()
    {
        PlayerPrefs.SetInt(SaveData_Manager.KEY_STONE_SELECT, (int)eSerect_Stone.uBLACK);
        _Serect_Stone_Mark.gameObject.transform.position = new Vector3(215, 945, 0);
    }

    /// <summary>
    /// 黒を選んだらPlayerPrefsに白をセット
    /// </summary>
    public void Select_White()
    {
        PlayerPrefs.SetInt(SaveData_Manager.KEY_STONE_SELECT, (int)eSerect_Stone.uWHITE);

        _Serect_Stone_Mark.gameObject.transform.position = new Vector3(500, 945, 0);
    }
    /// <summary>
    /// CPUのレベルセレクト・プルダウンメニューで選択
    /// </summary>
    public void UI_CPU_Level_Select()
    {
        var selectnow = _CPU_Level_Dropdown.value;

        switch ((eLevel_Select)selectnow)
        {
            case eLevel_Select.uLevel1:
                PlayerPrefs.SetInt(SaveData_Manager.KEY_CPU_LEVEL, (int)eLevel_Select.uLevel1);
                Debug.Log("レベル1を設定しました");
                break;

            case eLevel_Select.uLevel2:
                PlayerPrefs.SetInt(SaveData_Manager.KEY_CPU_LEVEL, (int)eLevel_Select.uLevel2);
                Debug.Log("レベル2を設定しました");
                break;

            case eLevel_Select.uLevel3:
                PlayerPrefs.SetInt(SaveData_Manager.KEY_CPU_LEVEL, (int)eLevel_Select.uLevel3);
                Debug.Log("レベル3を設定しました");
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// ゲームスタートボタンの開始
    /// </summary>
    public void GameStart()
    {
        SceneManager.LoadScene("3_Reversi_GameScene");
        _GamePlay_PopupPanel.SetActive(false);
    }

    /// <summary>
    /// ☓ボタンを押すとポップアップが非表示になる・今後DoTweenでアニメーションを入れる予定
    /// </summary>
    public void Cancel()
    {
        _GamePlay_PopupPanel.SetActive(false);
    }

    /// <summary>
    /// ポップアップの表示
    /// </summary>
    public void TapPlayButton()
    {
        Select_Black();
        _GamePlay_PopupPanel.SetActive(true);
    }
}
