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
    private SquadManager attached_squad; // Reference.


    public void Init(string _squad_name, ref SquadManager _squad)
    {
        squad_text.text = _squad_name;
        attached_squad = _squad;

        Deselect();
    }


    void Update()
    {
        if (attached_squad.num_squaddies == 0)
        {
            Destroy(this.gameObject);
            return;
        }

        UpdateUnitCount();
        HandleSelection();
    }


    void UpdateUnitCount()
    {
        int prev_value = int.Parse(num_units_text.text);

        if (attached_squad.num_squaddies > prev_value)
            fade.FadeColor(Color.green, EvaluateCurrentColor(), fade_time); // Unit gained.
        else if (attached_squad.num_squaddies < prev_value)
            fade.FadeColor(Color.red, EvaluateCurrentColor(), fade_time); // Unit lost.

        num_units_text.text = attached_squad.num_squaddies.ToString();
    }


    void HandleSelection()
    {
        if (attached_squad.selected && !selected)
            Select();
        
        if (!attached_squad.selected && selected)
            Deselect();
    }


    void Select()
    {
        bg.color = select_color;
        selected = true;
    }


    void Deselect()
    {
        bg.color = deselect_color;
        selected = false;

        fade.CancelFade();
    }


    Color EvaluateCurrentColor()
    {
        return selected ? select_color : deselect_color;
    }

}
