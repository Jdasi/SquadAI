using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContextType
{
    NONE,
    FLOOR,
    COVER,
    ATTACK,
    HACK
}

/// <summary>
/// Contains contextual information which can be used to drive other game systems.
/// This information is recorded based on the current PerspectiveMode: mouse position 
/// in Tactical mode, and the screen center in FPS mode.
/// </summary>
public class ContextScanner : MonoBehaviour
{
    public CurrentContext current_context = new CurrentContext();   // The current context information.
    public Transform indicator_transform { get { return context_indicator.transform; } }

    [Header("Parameters")]
    [SerializeField] float dist_from_first_ray;                     // How far from a surface the downward raycast should be performed to check for the floor.
    [SerializeField] float dist_from_second_ray;                    // How far above the floor the move indicator should be shown.
    [SerializeField] LayerMask hit_layers;                          // The layers that represent different contextual options.
    [SerializeField] float scan_interval = 0.005f;                  // How often the current context is updated.

    [Header("Layers")]
    [SerializeField] string floor_layer = "Floor";
    [SerializeField] string wall_layer = "Wall";
    [SerializeField] string damageable_layer = "Damageable";
    [SerializeField] string hackable_layer = "Hackable";

    [Header("References")]
    [SerializeField] Transform fps_transform;                       // Transform of the FPS camera.
    [SerializeField] Transform tactical_transform;                  // Transform of the Tactical camera.
    [SerializeField] ContextIndicator context_indicator;            // Reference to the class that encapsulates the contextual indicators.

    private int floor_layer_value;
    private int wall_layer_value;
    private int damageable_layer_value;
    private int hackable_layer_value;


    /// <summary>
    /// Should be called when an order is about to be issued.
    /// </summary>
    /// <param name="_faction">Determines the current faction being ordered.</param>
    public void Activate(FactionSettings _faction)
    {
        Deactivate();

        current_context.current_faction = _faction;
        InvokeRepeating("ScanContext", 0, scan_interval);
    }


    /// <summary>
    /// Should be called once an order has been issued.
    /// </summary>
    public void Deactivate()
    {
        CancelInvoke("ScanContext");

        current_context.type = ContextType.NONE;
        current_context.current_faction = null;

        context_indicator.ChangeIndicator(current_context.type);
    }


    void Start()
    {
        Deactivate();

        floor_layer_value = LayerMask.NameToLayer(floor_layer);
        wall_layer_value = LayerMask.NameToLayer(wall_layer);
        damageable_layer_value = LayerMask.NameToLayer(damageable_layer);
        hackable_layer_value = LayerMask.NameToLayer(hackable_layer);
    }


    /// <summary>
    /// Called every evaluation interval to update the current context.
    /// </summary>
    void ScanContext()
    {
        RaycastHit first_hit;
        bool first_ray_success;

        Raycast(out first_hit, out first_ray_success); // Try to hit a contextual object.
        EvaluateContext(first_ray_success, first_hit); // Evaluate the hit.

        if (current_context.type != ContextType.NONE)
        {
            ProcessContext(first_hit);
        }

        context_indicator.ChangeIndicator(current_context.type);
    }


    /// <summary>
    /// Performs a raycast from either the screen or the fps camera, based on the current PerspectiveMode.
    /// The results are transcribed into the passed hit and bool variables.
    /// </summary>
    /// <param name="_hit">RaycastHit structure to store hit details.</param>
    /// <param name="_ray_success">Boolean to describe raycast success.</param>
    void Raycast(out RaycastHit _hit, out bool _ray_success)
    {
        _hit = new RaycastHit();
        _ray_success = false;

        switch (GameManager.scene.perspective_manager.perspective)
        {
            case PerspectiveMode.FPS:
            {
                _ray_success = Physics.Raycast(fps_transform.position, fps_transform.forward,
                    out _hit, Mathf.Infinity, hit_layers);
            } break;

            case PerspectiveMode.TACTICAL:
            {
                Ray ray = JHelper.main_camera.ScreenPointToRay(Input.mousePosition);
                _ray_success = Physics.Raycast(ray, out _hit, Mathf.Infinity, hit_layers);
            } break;
        }
    }


    /// <summary>
    /// Determines the current context, based on the passed RaycastHit.
    /// </summary>
    /// <param name="_hit_successful">Was the raycast successful?</param>
    /// <param name="_first_hit">Details of the initial raycast.</param>
    void EvaluateContext(bool _hit_successful, RaycastHit _first_hit)
    {
        UpdateContextValues();

        if (_hit_successful)
        {
            current_context.indicator_normal = _first_hit.normal;
            current_context.indicator_hit = _first_hit.transform;

            if (_first_hit.collider.gameObject.layer == floor_layer_value)
            {
                current_context.type = ContextType.FLOOR;
            }
            else if (_first_hit.normal != Vector3.up &&
                     _first_hit.collider.gameObject.layer == wall_layer_value)
            {
                current_context.type = ContextType.COVER;
            }
            else if (_first_hit.collider.gameObject.layer == damageable_layer_value)
            {
                current_context.type = ContextType.ATTACK;
            }
            else if (_first_hit.collider.gameObject.layer == hackable_layer_value)
            {
                current_context.type = ContextType.HACK;
            }
            else
            {
                ResetContext();
            }
        }
        else
        {
            ResetContext();
        }
    }


    void UpdateContextValues()
    {
        current_context.indicator_position = context_indicator.transform.position;
        current_context.indicator_forward = context_indicator.transform.forward;
        current_context.indicator_up = context_indicator.transform.up;
        current_context.indicator_right = context_indicator.transform.right;
    }


    void ResetContext()
    {
        current_context.type = ContextType.NONE;
        current_context.indicator_hit = null;
        current_context.indicator_normal = Vector3.zero;
    }


    /// <summary>
    /// Updates the position of the context indicator based on the context.
    /// Sometimes additional tests are performed to determine the validity of the current context.
    /// </summary>
    /// <param name="_first_hit"></param>
    void ProcessContext(RaycastHit _first_hit)
    {
        switch (current_context.type)
        {
            case ContextType.FLOOR:
            {
                ProcessFloorContext(_first_hit);
            } break;

            case ContextType.COVER:
            {
                ProcessCoverContext(_first_hit);
            } break;

            case ContextType.ATTACK:
            {
                context_indicator.SetScreenPosition(Input.mousePosition);
            } break;

            case ContextType.HACK:
            {
                context_indicator.SetScreenPosition(Input.mousePosition);
            } break;
        }
    }


    void ProcessFloorContext(RaycastHit _hit)
    {
        context_indicator.transform.position = _hit.point + (_hit.normal * dist_from_first_ray);

        Vector3 pos = transform.position;
        pos.y = 0;

        context_indicator.transform.rotation = Quaternion.LookRotation(_hit.normal);
    }


    /// <summary>
    /// Performs various checks to assess the legitimacy of the cover context hit.
    /// The context type is set to NONE if any of the tests fail.
    /// </summary>
    /// <param name="_hit"></param>
    void ProcessCoverContext(RaycastHit _hit)
    {
        // Find Floor from Wall.
        RaycastHit second_hit;
        bool ray_2 = Physics.Raycast(_hit.point + (_hit.normal * dist_from_first_ray),
            -Vector3.up, out second_hit, Mathf.Infinity, 1 << floor_layer_value | 1 << wall_layer_value);

        if (second_hit.collider != null && second_hit.collider.gameObject.layer == floor_layer_value)
        {
            context_indicator.transform.position = second_hit.point + (Vector3.up * dist_from_second_ray);
            context_indicator.transform.rotation = Quaternion.LookRotation(_hit.normal);
            context_indicator.transform.Rotate(0, -180, 0);

            // Check for overhangs.
            if (!Physics.Raycast(second_hit.point + (Vector3.up * dist_from_second_ray), -_hit.normal, 1,
                1 << floor_layer_value | 1 << wall_layer_value))
            {
                current_context.type = ContextType.NONE;
            }
        }
        else
        {
            current_context.type = ContextType.NONE;
        }
    }

}
