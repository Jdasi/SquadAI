using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RaycastElement
{
    public string name;

    public Vector3 offset;
    public Vector3 direction;
    public float distance;
    public Color color;
    public LayerMask hit_layers;

    public CustomEvents.RaycastHitEvent fire_events;

}


public class RaycastArray : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float update_delay;
    [SerializeField] RaycastElement[] rays;


    void Start()
    {
        InvokeRepeating("ProcessArray", 0, update_delay);
    }


    void ProcessArray()
    {
        foreach (RaycastElement ray in rays)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position + ray.offset, ray.direction, out hit,
                ray.distance, ray.hit_layers);

            ray.fire_events.Invoke(hit);
        }
    }


    void OnDrawGizmos()
    {
        if (rays == null)
            return;

        foreach (RaycastElement ray in rays)
        {
            Gizmos.color = ray.color;

            Vector3 from = transform.position + ray.offset;
            Vector3 to = from + (ray.direction * ray.distance);

            Gizmos.DrawLine(from, to); 
            Gizmos.DrawSphere(to, 0.1f);
        }
    }

}
