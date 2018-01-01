using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirstPersonMouseLook : MonoBehaviour
{
    public bool mouse_locked = true;                                // Is mouse look enabled?

    [Header("Parameters")]
    [SerializeField] float horizontal_look_sensitivity = 150.0f;    // Sensitivity of the mouse X axis.
    [SerializeField] float vertical_look_sensitivity = 100.0f;      // Sensitivity of the mouse Y axis.
    [SerializeField] bool y_flipped;                                // Should the mouse Y axis be flipped?
    [SerializeField] UnityEvent look_enabled_events;                // Events that fire when mouse look is enabled.
    [SerializeField] UnityEvent look_disabled_events;               // Events that fire when mouse look is disabled.

    [Header("References")]
    [SerializeField] Transform x_rotate_transform;
    [SerializeField] Transform y_rotate_transform;

    private float pan_horizontal;
    private float pan_vertical;


	void Start()
    {
        MouseLockEvents();
	}
	

	void Update()
    {
        HandleMouseLookToggle();

        if (mouse_locked)
            HandleMouseLook();

        Cursor.visible = !mouse_locked;
        Cursor.lockState = mouse_locked ? CursorLockMode.Locked : CursorLockMode.None;
	}


    void HandleMouseLookToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mouse_locked = !mouse_locked;
            MouseLockEvents();
        }
    }


    void HandleMouseLook()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");

        pan_horizontal +=  horizontal * Time.deltaTime * horizontal_look_sensitivity;
        pan_vertical -= (!y_flipped ? vertical : -vertical) * Time.deltaTime * vertical_look_sensitivity;

        pan_vertical = Mathf.Clamp(pan_vertical, -90, 90);

        x_rotate_transform.rotation = Quaternion.Euler(0, pan_horizontal, 0);
        y_rotate_transform.rotation = Quaternion.Euler(pan_vertical, x_rotate_transform.eulerAngles.y, 0);
    }


    void MouseLockEvents()
    {
        if (mouse_locked)
        {
            look_enabled_events.Invoke();
        }
        else
        {
            look_disabled_events.Invoke();
        }
    }

}
