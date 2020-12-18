using UnityEngine;
using UnityEngine.UI;

public class UI_CPU_Select_Color : MonoBehaviour
{
    [SerializeField]
    Sprite[] UI_StoneColor = new Sprite[2];

    public void CPU_UIStoneStart(GridManager.estoneState selectStone)
    {
        if (selectStone == GridManager.estoneState.BLACK)
        {
            this.GetComponent<Image>().sprite = UI_StoneColor[(int)UI_Managaer.eUIStone_color.eUI_WHITE];
        }
        else
        {
            this.GetComponent<Image>().sprite = UI_StoneColor[(int)UI_Managaer.eUIStone_color.eUI_BLACK];
        }
    }
}
