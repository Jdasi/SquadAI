using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoverPoint
{
    public Vector3 position;
    public Vector3 normal;
    public bool occupied;
    public float weighting;

    private CoverPointSettings settings;
    private float last_scan_timestamp;


    public CoverPoint(CoverPointSettings _settings)
    {
        settings = _settings;
    }


    public void DetermineOccupiedStatus()
    {
        if (Time.time < last_scan_timestamp + settings.scan_delay)
            return;

        var hits = Physics.OverlapSphere(position, settings.scan_radius, settings.scan_layers);
        occupied = hits.Length > 0;

        last_scan_timestamp = Time.time;
    }

}
