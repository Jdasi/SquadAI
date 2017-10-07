using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventForwarder : MonoBehaviour
{
    [Header("Enter Events")]
    [SerializeField] CustomEvents.CollisionEvent collision_enter_events;
    [SerializeField] CustomEvents.ColliderEvent trigger_enter_events;
    [SerializeField] CustomEvents.Collision2DEvent collision_enter_2d_events;
    [SerializeField] CustomEvents.Collider2DEvent trigger_enter_2d_events;

    [Header("Exit Events")]
    [SerializeField] CustomEvents.CollisionEvent collision_exit_events;
    [SerializeField] CustomEvents.ColliderEvent trigger_exit_events;
    [SerializeField] CustomEvents.Collision2DEvent collision_exit_2d_events;
    [SerializeField] CustomEvents.Collider2DEvent trigger_exit_2d_events;


    // ENTER EVENTS -----------------------------------------------------------
    void OnCollisionEnter(Collision _other)
    {
        collision_enter_events.Invoke(_other);
    }


    void OnTriggerEnter(Collider _other)
    {
        trigger_enter_events.Invoke(_other);
    }


    void OnCollisionEnter2D(Collision2D _other)
    {
        collision_enter_2d_events.Invoke(_other);
    }


    void OnTriggerEnter2D(Collider2D _other)
    {
        trigger_enter_2d_events.Invoke(_other);
    }


    // EXIT EVENTS ------------------------------------------------------------
    void OnCollisionExit(Collision _other)
    {
        collision_exit_events.Invoke(_other);
    }


    void OnTriggerExit(Collider _other)
    {
        trigger_exit_events.Invoke(_other);
    }


    void OnCollisionExit2D(Collision2D _other)
    {
        collision_exit_2d_events.Invoke(_other);
    }


    void OnTriggerExit2D(Collider2D _other)
    {
        Debug.Log(this.name + " OnTriggerEnter: " + _other.name);
        trigger_exit_2d_events.Invoke(_other);
    }

}
