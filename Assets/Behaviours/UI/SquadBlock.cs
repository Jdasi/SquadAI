using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadBlock : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] Color select_color;
    [SerializeField] Color deselect_color;
    [SerializeField] float fade_time;

    [Header("References")]
    [SerializeField] Image bg;
    [SerializeField] Text squad_text;
    [SerializeField] Text num_units_text;
    [SerializeField] FadableGraphic fade;

    private bool selected;


    public void Init(int _squad_number)
    {
        bg.color = deselect_color;
        squad_text.text = "Squad " + (_squad_number + 1).ToString();
    }


    public void Select()
    {
        bg.color = select_color;
        selected = true;
    }


    public void Deselect()
    {
        bg.color = deselect_color;
        selected = false;

        fade.CancelFade();
    }


    public void UpdateUnitCount(int _unit_count)
    {
        int prev_value = int.Parse(num_units_text.text);

        if (_unit_count > prev_value)
            fade.FadeColor(Color.green, EvaluateCurrentColor(), fade_time); // Unit gained.
        else if (_unit_count < prev_value)
            fade.FadeColor(Color.red, EvaluateCurrentColor(), fade_time); // Unit lost.

        num_units_text.text = _unit_count.ToString();
    }


    Color EvaluateCurrentColor()
    {
        return selected ? select_color : deselect_color;
    }

}
