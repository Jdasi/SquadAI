using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Squad Settings")]
public class SquadSettings : ScriptableObject
{
    public int max_squaddies;
    public float squaddie_size;
    public float squaddie_spacing;
    public float cover_search_radius;

}
