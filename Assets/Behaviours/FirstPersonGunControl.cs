using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FirstPersonGunControl : MonoBehaviour
{
    public bool cycle;

    [Header("Parameters")]
    [SerializeField] float acceleration_speed;
    [SerializeField] float deceleration_speed;
    [SerializeField] float max_cycle_speed;
    [SerializeField] float shoot_delay;

    [Header("References")]
    [SerializeField] Transform gun_cycler;
    [SerializeField] Transform shot_point;
    [SerializeField] Transform raycast_transform;
    [SerializeField] GameObject shot_particle;
    [SerializeField] GameObject ricochet_particle;

    private float current_cycling_speed;
    private bool can_shoot = true;
    private float shoot_timer;


    public void EnableShooting()
    {
        can_shoot = true;
    }


    public void DisableShooting()
    {
        can_shoot = false;
    }


    void Update()
    {
        HandleCycling();
        HandleShooting();
    }


    void HandleCycling()
    {
        cycle = Input.GetMouseButton(0);

        if (cycle && can_shoot)
        {
            current_cycling_speed += acceleration_speed * Time.deltaTime;
        }
        else
        {
            if (current_cycling_speed > 0)
            {
                current_cycling_speed -= deceleration_speed * Time.deltaTime;
            }
            else
            {
                current_cycling_speed = 0;
            }
        }

        current_cycling_speed = Mathf.Clamp(current_cycling_speed, 0, max_cycle_speed);
        gun_cycler.Rotate(0, 0, -current_cycling_speed);
    }


    void HandleShooting()
    {
        if (current_cycling_speed >= max_cycle_speed)
        {
            shoot_timer += Time.deltaTime;

            if (shoot_timer >= shoot_delay)
            {
                shoot_timer = 0;
                HandleShot();
            }
        }
        else
        {
            shoot_timer = 0;
        }
    }


    void HandleShot()
    {
        var shot_clone = Instantiate(shot_particle, shot_point.position,
            Quaternion.LookRotation(shot_point.forward));

        RaycastHit hit;
        Physics.Raycast(raycast_transform.position, raycast_transform.forward,
            out hit, Mathf.Infinity);

        if (hit.collider != null)
        {
            var ricochet_clone = Instantiate(ricochet_particle, hit.point,
                Quaternion.LookRotation(hit.normal));
        }
    }

}
