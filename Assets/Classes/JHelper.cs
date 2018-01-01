using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JHelper
{
    private static Camera main_camera_;


    public static Camera main_camera
    {
        get
        {
            if (main_camera_ == null || Camera.current != main_camera_)
                main_camera_ = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            return main_camera_;
        }
    }


    public static void SetLayerRecursive(GameObject _obj, int _layer)
    {
        _obj.layer = _layer;

        foreach (Transform child in _obj.transform)
            SetLayerRecursive(child.gameObject, _layer);
    }


    public static bool SameFaction(FactionSettings _a, FactionSettings _b)
    {
        if (_a == null || _b == null)
            return false;

        if (_a.faction == _b.faction)
            return true;

        return false;
    }


    public static bool SameFaction(SquaddieAI _a, SquaddieAI _b)
    {
        if (_a == null || _b == null)
            return false;

        return SameFaction(_a.stats.faction_settings, _b.stats.faction_settings);
    }


    public static bool SameFaction(SquaddieAI _a, FactionSettings _b)
    {
        if (_a == null || _b == null)
            return false;

        return SameFaction(_a.stats.faction_settings, _b);
    }


    /// <summary>
    /// Test if a raycast from A can reach B without being blocked.
    /// </summary>
    /// <param name="_a">From position.</param>
    /// <param name="_b">To position.</param>
    /// <param name="_max_distance">Max distance of the raycast.</param>
    /// <param name="_blocking_layers">Object layers that can block the ray.</param>
    /// <returns>True if raycast from A reaches B, otherwise False.</returns>
    public static bool RaycastAToB(Vector3 _a, Vector3 _b, float _max_distance,
        LayerMask _blocking_layers)
    {
        Vector3 dir = (_b - _a).normalized;
        float dist = Vector3.Distance(_a, _b);

        RaycastHit hit;
        bool ray_blocked = Physics.Raycast(_a, dir, out hit,
            _max_distance, _blocking_layers);

        if (ray_blocked && hit.distance > dist)
            return true;

        return !ray_blocked && dist <= _max_distance;
    }


    public static bool RaycastMousePosToLayer(string _layer, out RaycastHit _hit)
    {
        Ray ray = JHelper.main_camera.ScreenPointToRay(Input.mousePosition);
        bool _ray_success = Physics.Raycast(ray, out _hit, Mathf.Infinity,
            1 << LayerMask.NameToLayer(_layer));

        return _ray_success;
    }


    public static Vector3 PosToCirclePos(Vector3 _pos, int _num_elems, int _elem_index, float _circle_size)
    {
        _elem_index = Mathf.Clamp(_elem_index, 0, _num_elems);

        float theta = ((2 * Mathf.PI) / _num_elems) * _elem_index;
        float x_pos = Mathf.Sin(theta);
        float z_pos = Mathf.Cos(theta);

        return _pos + (new Vector3(x_pos, 0, z_pos) * _circle_size);
    }

}
