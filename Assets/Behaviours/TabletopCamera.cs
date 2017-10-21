using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletopCamera : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float move_speed;
    [SerializeField] float sprint_speed_modifier;

    [Header("Startup")]
    [SerializeField] float starting_pitch;
    [SerializeField] float starting_zoom;
    [SerializeField] Vector3 origin_start;

    [Header("Sensitivity")]
    [SerializeField] float x_sensitivity;
    [SerializeField] float y_sensitivity;
    [SerializeField] float scroll_sensitivity;

    [Header("Restrictions")]
    [SerializeField] float min_pitch = 0;
    [SerializeField] float max_pitch = 90;
    [SerializeField] float min_zoom = 5;
    [SerializeField] float max_zoom = 20;

    private Vector3 origin;
    private Vector3 offset;

    private float yaw;
    private float pitch;

    private bool orbiting;
    private float speed_modifier;


    void Start()
    {
        pitch = starting_pitch;
        offset.z = starting_zoom;
        origin = origin_start;

        HandleOrbit();
    }

    
    void Update()
    {
        orbiting = Input.GetMouseButton(1);    
        speed_modifier = Input.GetButton("Sprint") ? sprint_speed_modifier : 1;

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
        yaw += Input.GetAxis("Mouse X") * Time.deltaTime * x_sensitivity;
        pitch += Input.GetAxis("Mouse Y") * Time.deltaTime * y_sensitivity;

        pitch = Mathf.Clamp(pitch, min_pitch, max_pitch);
    }


    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * speed_modifier * scroll_sensitivity;

        offset.z -= scroll * Time.deltaTime;
        offset.z = Mathf.Clamp(offset.z, min_zoom, max_zoom);
    }


    void HandleMovement()
    {
        Quaternion rot = Quaternion.Euler(0, yaw, 0);
        
        Vector3 forward_move = rot * Vector3.forward * move_speed;
        Vector3 side_move = rot * Vector3.right * move_speed;

        origin -= forward_move * Input.GetAxis("Vertical") * speed_modifier * Time.deltaTime;
        origin -= side_move * Input.GetAxis("Horizontal") * speed_modifier * Time.deltaTime;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(origin, 1);
    }

}
