using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings which describe a the appearance and logic of a cover point.
/// For information about a cover points position and normal, see the CoverPoint class.
/// </summary>
[CreateAssetMenu(menuName = "Custom/Cover Point Settings")]
public class CoverPointSettings : ScriptableObject
{
    public float scan_radius;           // Radius in which the cover point scans for occupants.
    public float scan_delay;            // Frequency of occupancy scans.
    public LayerMask scan_layers;       // Layers compared against when scanning.
    public Color occupied_color;        // Gizmo colour when occupied.
    public Color unoccupied_color;      // Gizmo colour when unoccupied.
}
