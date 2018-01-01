using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquaddieFaction
{
    BLU,
    RED
}

/// <summary>
/// Small container which describes the appearance and identity of a faction.
/// </summary>
[CreateAssetMenu(menuName = "Custom/Faction Settings")]
public class FactionSettings : ScriptableObject
{
    public SquaddieFaction faction;     // Enum identifier.
    public Material base_material;      // Material to be applied to squaddies of this faction.

    public Color arrow_color;           // Color of SquaddieCanvas elements.
    public Color text_color;            // Color of SquaddieCanvas elements.

}
