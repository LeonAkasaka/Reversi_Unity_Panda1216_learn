using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Select_Color : MonoBehaviour
{
    [SerializeField]
    Sprite[] UI_StoneColor = new Sprite[2];

    public void Play_UIStoneStart(estoneState selectStone)
    {
        if (selectStone == estoneState.BLACK)
        {
            this.GetComponent<Image>().sprite = UI_StoneColor[(int)UI_Managaer.eUIStone_color.eUI_BLACK];
        }
        else
        {

            this.GetComponent<Image>().sprite = UI_StoneColor[(int)UI_Managaer.eUIStone_color.eUI_WHITE];
        }
    }
}
