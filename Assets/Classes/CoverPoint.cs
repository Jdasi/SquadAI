using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the information of an individual cover point.
/// </summary>
[System.Serializable]
public class CoverPoint
{
    public Vector3 position;                // World space of the cover point.
    public Vector3 normal;                  // The normal of the surface this cover point applies to.
    public bool occupied;                   // Is this cover point occupied?
    public float weighting;                 // Used by TacticalAssessor in evaluation cycles.

    private CoverPointSettings settings;
    private float last_scan_timestamp;


    public CoverPoint(CoverPointSettings _settings)
    {
        settings = _settings;
    }


    /// <summary>
    /// Should be called frequently to prevent misinformation inside the TacticalAssessor.
    /// </summary>
    public void DetermineOccupiedStatus()
    {
        if (Time.time < last_scan_timestamp + settings.scan_delay)
            return;

        var hits = Physics.OverlapSphere(position, settings.scan_radius, settings.scan_layers);
        occupied = hits.Length > 0;

        last_scan_timestamp = Time.time;
    }

}
