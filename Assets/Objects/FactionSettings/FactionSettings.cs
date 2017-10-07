using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquaddieFaction
{
    BLU,
    RED
}

[CreateAssetMenu(menuName = "Custom/Faction Settings")]
public class FactionSettings : ScriptableObject
{
    public SquaddieFaction faction;
    public Material select_material;
    public Material deselect_material;

    public Color arrow_color;
    public Color text_color;

}
