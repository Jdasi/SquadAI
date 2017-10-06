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

}
