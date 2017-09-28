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
    public ContextType current_context { get; private set; }
    public Vector3 indicator_position { get { return context_indicator.transform.position; } }
    public Transform indicator_hit { get; private set; }

    [Header("Parameters")]
    [SerializeField] float dist_from_first_ray;
    [SerializeField] float dist_from_second_ray;
    [SerializeField] string floor_layer = "Floor";
    [SerializeField] string wall_layer = "Wall";

    [Header("References")]
    [SerializeField] Transform forward_transform;
    [SerializeField] GameObject context_indicator;
    [SerializeField] GameObject waypoint_indicator;
    [SerializeField] GameObject cover_indicator;

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
            out first_hit, Mathf.Infinity, 1 << floor_layer_value | 1 << wall_layer_value);

        EvaluateContext(first_ray_success, first_hit);

        if (current_context != ContextType.NONE)
        {
            ProcessContext(first_hit);
        }

        UpdateIndicators(first_hit);
    }


    void EvaluateContext(bool _hit_successful, RaycastHit _first_hit)
    {
        bool hit_valid = _hit_successful && _first_hit.normal != Vector3.down;

        if (!hit_valid)
        {
            ResetContext();
        }
        else if (_first_hit.normal == Vector3.up &&
                 _first_hit.collider.gameObject.layer == floor_layer_value)
        {
            current_context = ContextType.FLOOR;
            indicator_hit = _first_hit.transform;
        }
        else if (_first_hit.normal != Vector3.up &&
                 _first_hit.collider.gameObject.layer == wall_layer_value)
        {
            current_context = ContextType.COVER;
            indicator_hit = _first_hit.transform;
        }
        else
        {
            ResetContext();
        }
    }


    void ResetContext()
    {
        current_context = ContextType.NONE;
        indicator_hit = null;
    }


    void ProcessContext(RaycastHit first_hit)
    {
        switch (current_context)
        {
            case ContextType.FLOOR:
            {
                context_indicator.transform.position = first_hit.point + (first_hit.normal * dist_from_first_ray);

                Vector3 pos = transform.position;
                pos.y = 0;

                context_indicator.transform.rotation = Quaternion.LookRotation(context_indicator.transform.position - pos);
            } break;

            case ContextType.COVER:
            {
                // Find Floor from Wall.
                RaycastHit second_hit;
                bool ray_2 = Physics.Raycast(first_hit.point + (first_hit.normal * dist_from_first_ray),
                    -Vector3.up, out second_hit, Mathf.Infinity, 1 << floor_layer_value | 1 << wall_layer_value);

                if (second_hit.collider != null && second_hit.collider.gameObject.layer == floor_layer_value)
                {
                    context_indicator.transform.position = second_hit.point + (Vector3.up * dist_from_second_ray);
                    context_indicator.transform.rotation = Quaternion.LookRotation(first_hit.normal);
                    context_indicator.transform.Rotate(0, -180, 0);

                    // Check for overhangs.
                    if (!Physics.Raycast(second_hit.point + (Vector3.up * dist_from_second_ray), -first_hit.normal, 1,
                        1 << floor_layer_value | 1 << wall_layer_value))
                    {
                        current_context = ContextType.NONE;
                    }
                }
                else
                {
                    current_context = ContextType.NONE;
                }
            } break;
        }
    }


    void UpdateIndicators(RaycastHit _first_hit)
    {
        waypoint_indicator.SetActive(current_context == ContextType.FLOOR);
        cover_indicator.SetActive(current_context == ContextType.COVER);
    }

}
