﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContextType
{
    NONE,
    FLOOR,
    COVER,
    ATTACK,
    HACK
}

public class ContextScanner : MonoBehaviour
{
    public CurrentContext current_context = new CurrentContext();
    public Transform indicator_transform { get { return context_indicator.transform; } }

    [Header("Parameters")]
    [SerializeField] float dist_from_first_ray;
    [SerializeField] float dist_from_second_ray;
    [SerializeField] LayerMask hit_layers;
    [SerializeField] float scan_interval = 0.005f;

    [Header("Layers")]
    [SerializeField] string floor_layer = "Floor";
    [SerializeField] string wall_layer = "Wall";
    [SerializeField] string damageable_layer = "Damageable";
    [SerializeField] string hackable_layer = "Hackable";

    [Header("References")]
    [SerializeField] Transform fps_transform;
    [SerializeField] Transform tactical_transform;
    [SerializeField] ContextIndicator context_indicator;    

    private int floor_layer_value;
    private int wall_layer_value;
    private int damageable_layer_value;
    private int hackable_layer_value;


    public void Activate(FactionSettings _faction)
    {
        Deactivate();

        current_context.current_faction = _faction;
        InvokeRepeating("ScanContext", 0, scan_interval);
    }


    public void Deactivate()
    {
        CancelInvoke("ScanContext");

        current_context.type = ContextType.NONE;
        current_context.current_faction = null;

        context_indicator.ChangeIndicator(current_context.type);
    }


    void Start()
    {
        Deactivate();

        floor_layer_value = LayerMask.NameToLayer(floor_layer);
        wall_layer_value = LayerMask.NameToLayer(wall_layer);
        damageable_layer_value = LayerMask.NameToLayer(damageable_layer);
        hackable_layer_value = LayerMask.NameToLayer(hackable_layer);
    }


    void ScanContext()
    {
        RaycastHit first_hit;
        bool first_ray_success;

        Raycast(out first_hit, out first_ray_success);
        EvaluateContext(first_ray_success, first_hit);

        if (current_context.type != ContextType.NONE)
        {
            ProcessContext(first_hit);
        }

        context_indicator.ChangeIndicator(current_context.type);
    }


    void Raycast(out RaycastHit _hit, out bool _ray_success)
    {
        _hit = new RaycastHit();
        _ray_success = false;

        switch (GameManager.scene.perspective_manager.perspective)
        {
            case PerspectiveMode.FPS:
            {
                _ray_success = Physics.Raycast(fps_transform.position, fps_transform.forward,
                    out _hit, Mathf.Infinity, hit_layers);
            } break;

            case PerspectiveMode.TACTICAL:
            {
                Ray ray = JHelper.main_camera.ScreenPointToRay(Input.mousePosition);
                _ray_success = Physics.Raycast(ray, out _hit, Mathf.Infinity, hit_layers);
            } break;
        }
    }


    void EvaluateContext(bool _hit_successful, RaycastHit _first_hit)
    {
        UpdateContextValues();

        if (_hit_successful)
        {
            current_context.indicator_normal = _first_hit.normal;
            current_context.indicator_hit = _first_hit.transform;

            if (_first_hit.collider.gameObject.layer == floor_layer_value)
            {
                current_context.type = ContextType.FLOOR;
            }
            else if (_first_hit.normal != Vector3.up &&
                     _first_hit.collider.gameObject.layer == wall_layer_value)
            {
                current_context.type = ContextType.COVER;
            }
            else if (_first_hit.collider.gameObject.layer == damageable_layer_value)
            {
                current_context.type = ContextType.ATTACK;
            }
            else if (_first_hit.collider.gameObject.layer == hackable_layer_value)
            {
                current_context.type = ContextType.HACK;
            }
            else
            {
                ResetContext();
            }
        }
        else
        {
            ResetContext();
        }
    }


    void UpdateContextValues()
    {
        current_context.indicator_position = context_indicator.transform.position;
        current_context.indicator_forward = context_indicator.transform.forward;
        current_context.indicator_up = context_indicator.transform.up;
        current_context.indicator_right = context_indicator.transform.right;
    }


    void ResetContext()
    {
        current_context.type = ContextType.NONE;
        current_context.indicator_hit = null;
        current_context.indicator_normal = Vector3.zero;
    }


    void ProcessContext(RaycastHit _first_hit)
    {
        switch (current_context.type)
        {
            case ContextType.FLOOR:
            {
                context_indicator.transform.position = _first_hit.point + (_first_hit.normal * dist_from_first_ray);

                Vector3 pos = transform.position;
                pos.y = 0;

                context_indicator.transform.rotation = Quaternion.LookRotation(_first_hit.normal);
            } break;

            case ContextType.COVER:
            {
                // Find Floor from Wall.
                RaycastHit second_hit;
                bool ray_2 = Physics.Raycast(_first_hit.point + (_first_hit.normal * dist_from_first_ray),
                    -Vector3.up, out second_hit, Mathf.Infinity, 1 << floor_layer_value | 1 << wall_layer_value);

                if (second_hit.collider != null && second_hit.collider.gameObject.layer == floor_layer_value)
                {
                    context_indicator.transform.position = second_hit.point + (Vector3.up * dist_from_second_ray);
                    context_indicator.transform.rotation = Quaternion.LookRotation(_first_hit.normal);
                    context_indicator.transform.Rotate(0, -180, 0);

                    // Check for overhangs.
                    if (!Physics.Raycast(second_hit.point + (Vector3.up * dist_from_second_ray), -_first_hit.normal, 1,
                        1 << floor_layer_value | 1 << wall_layer_value))
                    {
                        current_context.type = ContextType.NONE;
                    }
                }
                else
                {
                    current_context.type = ContextType.NONE;
                }
            } break;

            case ContextType.ATTACK:
            {
                context_indicator.SetScreenPosition(Input.mousePosition);
            } break;

            case ContextType.HACK:
            {
                context_indicator.SetScreenPosition(Input.mousePosition);
            } break;
        }
    }

}
