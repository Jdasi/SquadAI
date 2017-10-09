using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContextType
{
    NONE,
    FLOOR,
    COVER
}

public class ContextScanner : MonoBehaviour
{
    public CurrentContext current_context = new CurrentContext();

    [Header("Parameters")]
    [SerializeField] float dist_from_first_ray;
    [SerializeField] float dist_from_second_ray;
    [SerializeField] LayerMask hit_layers;
    [SerializeField] string floor_layer = "Floor";
    [SerializeField] string wall_layer = "Wall";

    [Header("References")]
    [SerializeField] Transform forward_transform;
    [SerializeField] ContextIndicator context_indicator;

    private int floor_layer_value;
    private int wall_layer_value;


    void Start()
    {
        floor_layer_value = LayerMask.NameToLayer(floor_layer);
        wall_layer_value = LayerMask.NameToLayer(wall_layer);

        InvokeRepeating("Raycast", 0, 0.005f);
    }


    void Raycast()
    {
        // Find Wall or Floor.
        RaycastHit first_hit;
        bool first_ray_success = Physics.Raycast(forward_transform.position, forward_transform.forward,
            out first_hit, Mathf.Infinity, hit_layers);

        EvaluateContext(first_ray_success, first_hit);

        if (current_context.type != ContextType.NONE)
        {
            ProcessContext(first_hit);
        }

        context_indicator.ChangeIndicator(current_context.type);
    }


    void EvaluateContext(bool _hit_successful, RaycastHit _first_hit)
    {
        UpdateContextValues();

        if (_hit_successful)
        {
            current_context.indicator_normal = _first_hit.normal;
            current_context.indicator_hit = _first_hit.transform;

            if (_first_hit.normal == Vector3.up &&
                _first_hit.collider.gameObject.layer == floor_layer_value)
            {
                current_context.type = ContextType.FLOOR;
            }
            else if (_first_hit.normal != Vector3.up &&
                     _first_hit.collider.gameObject.layer == wall_layer_value)
            {
                current_context.type = ContextType.COVER;
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

                context_indicator.transform.rotation = Quaternion.LookRotation(context_indicator.transform.position - pos);
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
        }
    }


    void OnEnable()
    {
        InvokeRepeating("Raycast", 0, 0.005f);
    }


    void OnDisable()
    {
        CancelInvoke("Raycast");

        current_context.type = ContextType.NONE;
        context_indicator.ChangeIndicator(current_context.type);
    }

}
