using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverScanner : MonoBehaviour
{
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
        bool ray_1 = Physics.Raycast(forward_transform.position, forward_transform.forward,
            out first_hit, Mathf.Infinity, 1 << floor_layer_value | 1 << wall_layer_value);

        bool floor_hit = false;
        bool wall_hit = false;

        if (ray_1)
        {
            bool hit_valid = first_hit.normal != Vector3.down;

            floor_hit = hit_valid && first_hit.collider.gameObject.layer == floor_layer_value;
            wall_hit = hit_valid && !floor_hit && first_hit.normal != Vector3.up;

            context_indicator.transform.rotation = Quaternion.LookRotation(first_hit.normal);

            if (floor_hit)
            {
                context_indicator.transform.position = first_hit.point + (first_hit.normal * dist_from_first_ray);
                context_indicator.transform.Rotate(180, 0, 0);
            }
            else if (wall_hit)
            {
                // Find Floor from Wall.
                RaycastHit second_hit;
                bool ray_2 = Physics.Raycast(first_hit.point + (first_hit.normal * dist_from_first_ray),
                    -Vector3.up, out second_hit, Mathf.Infinity, 1 << floor_layer_value | 1 << wall_layer_value);

                if (second_hit.collider != null && second_hit.collider.gameObject.layer == floor_layer_value)
                {
                    context_indicator.transform.position = second_hit.point + (Vector3.up * dist_from_second_ray);
                    context_indicator.transform.Rotate(0, -180, 0);

                    // Check for overhangs.
                    if (!Physics.Raycast(second_hit.point + (Vector3.up * dist_from_second_ray), -first_hit.normal, 1,
                        1 << floor_layer_value | 1 << wall_layer_value))
                    {
                        wall_hit = false;
                    }
                }
                else
                {
                    wall_hit = false;
                }
            }

        }

        waypoint_indicator.SetActive(floor_hit);
        cover_indicator.SetActive(wall_hit);
    }


}
