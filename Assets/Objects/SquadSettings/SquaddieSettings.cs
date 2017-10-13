using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad/Squaddie Settings")]
public class SquaddieSettings : ScriptableObject
{
    [Header("Navigation")]
    public float cover_search_radius;

    [Header("Senses")]
    public float field_of_view;
    public float sight_distance;

    [Header("Combat")]
    public float engage_distance;

}
