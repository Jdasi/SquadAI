using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquaddieCanvas : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image arrow;
    [SerializeField] Text state_display;


    public void Init(FactionSettings _faction_settings)
    {
        arrow.color = _faction_settings.arrow_color;
        state_display.color = _faction_settings.text_color;
    }



    public void UpdateStateDisplay(string _str)
    {
        state_display.text = _str;
    }

}
