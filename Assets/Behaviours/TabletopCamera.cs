using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletopCamera : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float move_speed;
    [SerializeField] float sprint_speed_modifier;
    [SerializeField] float walk_speed_modifier;

    [Header("Startup")]
    [SerializeField] float starting_pitch;
    [SerializeField] float starting_zoom;
    [SerializeField] Vector3 origin;

    [Header("Sensitivity")]
    [SerializeField] float x_sensitivity;
    [SerializeField] float y_sensitivity;
    [SerializeField] float scroll_sensitivity;

    [Header("Restrictions")]
    [SerializeField] float min_pitch = 0;
    [SerializeField] float max_pitch = 90;
    [SerializeField] float min_zoom = 5;
    [SerializeField] float max_zoom = 20;

    private Vector3 offset;

    private float yaw;
    private float pitch;

    private bool orbiting;
    private float speed_modifier;


    void Start()
    {
        pitch = starting_pitch;
        offset.z = starting_zoom;

        HandleOrbit();
    }

    
    void Update()
    {
        orbiting = Input.GetMouseButton(1);

        if (Input.GetButton("Sprint"))
        {
            speed_modifier = sprint_speed_modifier;
        }
        else if (Input.GetButton("Walk"))
        {
            speed_modifier = walk_speed_modifier;
        }
        else
        {
            speed_modifier = 1;
        }

        Cursor.visible = !orbiting;
        Cursor.lockState = orbiting ? CursorLockMode.Locked : CursorLockMode.None;

        if (orbiting)
            HandleOrbit();

        HandleZoom();
        HandleMovement();

        transform.position = origin + (Quaternion.Euler(pitch, yaw, 0) * offset);
        transform.LookAt(origin);
    }


    void HandleOrbit()
    {
        yaw += Input.GetAxis("Mouse X") * Time.unscaledDeltaTime * x_sensitivity;
        pitch += Input.GetAxis("Mouse Y") * Time.unscaledDeltaTime * y_sensitivity;

        pitch = Mathf.Clamp(pitch, min_pitch, max_pitch);
    }


    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * speed_modifier * scroll_sensitivity;

        offset.z -= scroll * Time.unscaledDeltaTime;
        offset.z = Mathf.Clamp(offset.z, min_zoom, max_zoom);
    }


    void HandleMovement()
    {
        Quaternion rot = Quaternion.Euler(0, yaw, 0);
        
        Vector3 forward_move = rot * Vector3.forward * move_speed;
        Vector3 side_move = rot * Vector3.right * move_speed;

        origin -= forward_move * Input.GetAxisRaw("Vertical") * speed_modifier * Time.unscaledDeltaTime;
        origin -= side_move * Input.GetAxisRaw("Horizontal") * speed_modifier * Time.unscaledDeltaTime;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(origin, 1);
    }

}
