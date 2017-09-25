﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMouseLook : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float horizontal_look_sensitivity = 150.0f;
    [SerializeField] float vertical_look_sensitivity = 100.0f;
    [SerializeField] bool y_flipped;

    [Header("References")]
    [SerializeField] Transform x_rotate_transform;
    [SerializeField] Transform y_rotate_transform;

    private float pan_horizontal;
    private float pan_vertical;


	void Start()
    {

	}
	

	void Update()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");

        pan_horizontal +=  horizontal * Time.deltaTime * horizontal_look_sensitivity;
        pan_vertical -= (!y_flipped ? vertical : -vertical) * Time.deltaTime * vertical_look_sensitivity;

        pan_vertical = Mathf.Clamp(pan_vertical, -90, 90);

        x_rotate_transform.rotation = Quaternion.Euler(0, pan_horizontal, 0);
        y_rotate_transform.rotation = Quaternion.Euler(pan_vertical, x_rotate_transform.eulerAngles.y, 0);
	}

}
