using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PerspectiveMode
{
    FPS,
    TACTICAL
}

public class PerspectiveManager : MonoBehaviour
{
    public PerspectiveMode perspective { get; private set; }

    [Header("Parameters")]
    [SerializeField] PerspectiveMode starting_perspective;
    [SerializeField] KeyCode toggle_perspective_key = KeyCode.P;

    [Header("References")]
    [SerializeField] GameObject player_obj;
    [SerializeField] GameObject tabletop_cam_obj;


    void Start()
    {
        perspective = starting_perspective;
        UpdatePerspectives();
    }


    void Update()
    {
        if (Input.GetKeyDown(toggle_perspective_key))
            TogglePerspective();
    }


    void TogglePerspective()
    {
        perspective = perspective == PerspectiveMode.FPS ?
            PerspectiveMode.TACTICAL : PerspectiveMode.FPS;

        UpdatePerspectives();
    }


    void UpdatePerspectives()
    {
        player_obj.SetActive(perspective == PerspectiveMode.FPS);
        tabletop_cam_obj.SetActive(perspective == PerspectiveMode.TACTICAL);
    }

}
