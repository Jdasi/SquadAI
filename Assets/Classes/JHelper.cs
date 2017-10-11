using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

}
