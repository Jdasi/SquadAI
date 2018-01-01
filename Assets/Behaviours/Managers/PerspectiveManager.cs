using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PerspectiveMode
{
    FPS,
    TACTICAL
}

/// <summary>
/// Handles the transition between First-Person and Tactical perspectives.
/// Other classes can specialise their behaviour based on the current perspective mode.
/// </summary>
public class PerspectiveManager : MonoBehaviour
{
    public PerspectiveMode perspective { get; private set; }

    [Header("Parameters")]
    [SerializeField] PerspectiveMode starting_perspective;
    [SerializeField] KeyCode toggle_perspective_key = KeyCode.P;

    [Header("References")]
    [SerializeField] GameObject player_obj;
    [SerializeField] GameObject tabletop_cam_obj;


    public bool FPSModeActive()
    {
        return perspective == PerspectiveMode.FPS;
    }


    public bool TacticalModeActive()
    {
        return perspective == PerspectiveMode.TACTICAL;
    }


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
        GameManager.scene.player_squad_control.OrderFinished();

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
