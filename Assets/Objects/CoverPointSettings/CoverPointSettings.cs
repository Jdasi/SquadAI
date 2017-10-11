using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Cover Point Settings")]
public class CoverPointSettings : ScriptableObject
{
    public float scan_radius;
    public float scan_delay;
    public LayerMask scan_layers;
    public Color occupied_color;
    public Color unoccupied_color;

}
