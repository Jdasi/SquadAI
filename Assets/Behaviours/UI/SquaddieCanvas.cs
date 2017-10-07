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


    public void UpdateStateDisplay(SquaddieState _state)
    {
        string state_str = "";

        switch (_state)
        {
            case SquaddieState.IDLE: state_str          = "IDLE"; break;
            case SquaddieState.MOVING: state_str        = "MOVING"; break;
            case SquaddieState.TAKING_COVER: state_str  = "TAKING COVER"; break;
            case SquaddieState.ENGAGING: state_str      = "ENGAGING ENEMY"; break;
        }

        state_display.text = state_str;
    }

}
