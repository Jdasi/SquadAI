﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainGun : MonoBehaviour
{
    public bool cycle;

    [Header("Parameters")]
    [SerializeField] float acceleration_speed;
    [SerializeField] float deceleration_speed;
    [SerializeField] float max_cycle_speed;
    [SerializeField] float shoot_delay;

    [Space]
    [SerializeField] float case_ejection_speed;
    [SerializeField] float case_ejection_variance;

    [Header("References")]
    [SerializeField] Transform gun_cycler;
    [SerializeField] Transform shot_point;
    [SerializeField] Transform case_ejection_point;
    [SerializeField] GameObject shot_particle_prefab;
    [SerializeField] GameObject ricochet_particle_prefab;
    [SerializeField] GameObject bullet_casing_prefab;

    private Transform raycast_transform;
    private float current_cycling_speed;
    private bool can_shoot = true;
    private float shoot_timer;


    public void Init(Transform _raycast_transform)
    {
        raycast_transform = _raycast_transform;
    }


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
        var shot_clone = Instantiate(shot_particle_prefab, shot_point.position,
            Quaternion.LookRotation(shot_point.forward));

        HitScan();
        EjectCasing();
    }


    void HitScan()
    {
        RaycastHit hit;
        Physics.Raycast(raycast_transform.position, raycast_transform.forward,
            out hit, Mathf.Infinity);

        if (hit.collider == null)
            return;

        var ricochet_clone = Instantiate(ricochet_particle_prefab, hit.point,
            Quaternion.LookRotation(hit.normal));
    }


    void EjectCasing()
    {
        var ejected_casing = Instantiate(bullet_casing_prefab, case_ejection_point.position,
            Quaternion.LookRotation(raycast_transform.forward));

        Vector3 ejection_velocity = case_ejection_point.right * case_ejection_speed;
        Vector3 variance = Vector3.one * (Random.Range(-case_ejection_speed, case_ejection_speed) * case_ejection_variance);
        ejection_velocity += variance;

        ejected_casing.GetComponent<Rigidbody>().velocity = ejection_velocity;
    }

}