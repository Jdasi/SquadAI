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
            if (main_camera_ == null)
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


    // Returns True if raycast from A reaches B, otherwise returns False.
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


    public static bool RaycastCameraToFloor(out RaycastHit _hit)
    {
        Ray ray = JHelper.main_camera.ScreenPointToRay(Input.mousePosition);
        bool _ray_success = Physics.Raycast(ray, out _hit, Mathf.Infinity,
            1 << LayerMask.NameToLayer("Floor"));

        return _ray_success;
    }

}
