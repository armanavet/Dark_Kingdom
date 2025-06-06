using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuPanelManager : MonoBehaviour
{
    [SerializeField] RectTransform UnderLine;
    //[SerializeField] Button AudioBT,VideoBT,GamePlayBT;





    public void a_BTAddUnderline(int underlineXPos)
    {
        UnderLine.position = new Vector2(underlineXPos, UnderLine.position.y);

    }
    public void b_BTChangeUnderlineSize(int width)
    {
        UnderLine.sizeDelta = new Vector2(width, UnderLine.sizeDelta.y);
            }
}
