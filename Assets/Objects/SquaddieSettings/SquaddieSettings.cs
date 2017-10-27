using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad/Squaddie Settings")]
public class SquaddieSettings : ScriptableObject
{
    [Header("Identity")]
    public string settings_name;
    public Sprite settings_icon;

    [Header("Navigation")]
    public float cover_search_radius;
    public float move_stop_distance;
    public float follow_stop_distance;

    [Header("Senses")]
    public float field_of_view;
    public float sight_distance;
    public float sense_delay;

    [Header("Combat")]
    public float minimum_engage_distance;
    public float maximum_engage_distance;
    public float accuracy_penalty;
    public float cover_preference;
    public float chase_range;

}
