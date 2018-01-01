using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The core driving system of an individual AI agent.
/// This behaviour handles the AI's state and working knowledge, and has functionality to 
/// update and query the AI's senses.
/// </summary>
public class SquaddieAI : MonoBehaviour
{
    [Header("Parameters")]
    public SquaddieSettings settings;                             // Describes the AI's capabilities.
    [SerializeField] State current_state;                         // Defines the AI's current mindset.
    [SerializeField] LayerMask sight_test_layers;                 // Layers that block the AI's line of sight.
    [SerializeField] float crouch_height;                         // The height reduction percentage of the AI when crouched.

    [Header("Knowledge")]
    public WorkingKnowledge knowledge = new WorkingKnowledge();   // A representation of the AI's known state of the world.

    [Header("References")]
    public NavMeshAgent nav;                                      // Enables AI pathfinding.
    public Transform view_point;                                  // Transform of the agent's eyes.
    public Transform torso_transform;                             // Transform of the agent's chest.
    public SquaddieStats stats;                                   // The AI's lifeforce and alliegence.

    [Space]
    [SerializeField] Outline[] outlines;                          // Used to control the outline visualisation around the agents form.
    [SerializeField] MeshRenderer[] meshes;                       // Used to texture the agent, based on faction.

    [Space]
    [SerializeField] Transform mesh_transform;                    // Used to crouch the agent.
    [SerializeField] Transform leg_transform;                     // Used to crouch the agent.
    [SerializeField] SquaddieCanvas squaddie_canvas;              // Visual representation of the AI's state.
    [SerializeField] SphereCollider scan_sphere;                  // Detection radius of the agent.

    private float original_height;
    private Vector3 original_leg_scale;

    private bool cover_forward;
    private bool cover_backward;
    private bool cover_left;
    private bool cover_right;


    /// <summary>
    /// Must be called after a squaddie is created to initialise key variables.
    /// </summary>
    /// <param name="_faction">The faction settings of the squaddie.</param>
    public void Init(FactionSettings _faction)
    {
        stats.faction_settings = _faction;
        squaddie_canvas.Init(_faction);

        foreach (MeshRenderer renderer in meshes)
            renderer.material = _faction.base_material;

        SetSelected(false);
    }


    public void SetSelected(bool _selected)
    {
        foreach (Outline outline in outlines)
        {
            if (outline == null)
                continue;

            outline.enabled = _selected;
        }
    }


    // ChaingunEquipper event.
    public void SetChainGun(ChainGun _gun)
    {
        knowledge.chain_gun = _gun;
    }


    public void LinkSquadSense(ref SquadSense _squad_sense)
    {
        knowledge.squad_sense = _squad_sense;
    }


    /// <summary>
    /// Causes the agent to transition to a new state.
    /// </summary>
    /// <param name="_target_state">The state to enter.</param>
    public void TransitionToState(State _target_state)
    {
        if (_target_state == null)
            return;

        OnStateExit(current_state);
        current_state = _target_state;
        OnStateEnter(current_state);
    }


    /// <summary>
    /// Clears all the agent's knowledge relating to orders.
    /// </summary>
    public void ResetOrderKnowledge()
    {
        knowledge.current_order = OrderType.NONE;
        knowledge.order_target = null;
        knowledge.order_waypoint = Vector3.zero;
        knowledge.order_console = null;

        nav.stoppingDistance = settings.move_stop_distance;
    }


    /// <summary>
    /// Issue the squaddie an order to move to the target location.
    /// </summary>
    /// <param name="_target">The target location.</param>
    public void IssueMoveCommand(Vector3 _target)
    {
        knowledge.current_order = OrderType.MOVE;

        knowledge.order_target = null;
        knowledge.order_waypoint = _target;

        nav.destination = _target;
    }


    /// <summary>
    /// Cause the agent to seek cover near the target location.
    /// </summary>
    /// <param name="_position">Location to seek cover near.</param>
    public void MoveToCoverNearPosition(Vector3 _position)
    {
        var cover_points = GameManager.scene.tactical_assessor.ClosestCoverPoints(
            _position, settings.cover_search_radius);

        if (cover_points.Count == 0)
            return;

        CoverPoint target_point = cover_points[0];
        nav.destination = target_point.position;
    }


    /// <summary>
    /// Cause the agent to seek a position that allows it to safely engage the target.
    /// </summary>
    /// <param name="_target">The flank target.</param>
    public void MoveToFlankEnemy(SquaddieAI _target)
    {
        var cover_points = GameManager.scene.tactical_assessor.FindFlankingPositions(
            this, _target, settings.cover_search_radius);

        if (cover_points.Count == 0)
        {
            nav.destination = _target.transform.position;
        }
        else
        {
            nav.destination = EvaluateUniqueCoverPoint(cover_points);
        }
    }


    
    Vector3 EvaluateUniqueCoverPoint(List<CoverPoint> _cover_points)
    {
        foreach (CoverPoint cover_point in _cover_points)
        {
            bool unique_pos = true;

            foreach (SquaddieAI ally in knowledge.squad_sense.squaddies)
            {
                if (ally == this || ally == null)
                    continue;

                if ((cover_point.position - ally.nav.destination).magnitude >
                    ally.settings.move_stop_distance + ally.nav.radius)
                {
                    continue;
                }

                unique_pos = false;
                break;
            }

            if (!unique_pos)
                continue;

            return cover_point.position;
        }

        return transform.position;
    }



    /// <summary>
    /// Tests the agent's sight to a position from its current location.
    /// </summary>
    /// <param name="_position">The sight location to test.</param>
    /// <returns>Returns true if it can see the position, otherwise returns false.</returns>
    public bool TestSightToPosition(Vector3 _position)
    {
        return JHelper.RaycastAToB(view_point.position, _position,
            settings.sight_distance, sight_test_layers);
    }


    /// <summary>
    /// Tests the agent's sight to a position, as if it were standing in a specific location.
    /// </summary>
    /// <param name="_standing_pos">The hypothetical standing location of the agent.</param>
    /// <param name="_position">The sight location to test.</param>
    /// <returns>Returns true if it can see the position, otherwise returns false.</returns>
    public bool TestSightToPosition(Vector3 _standing_pos, Vector3 _position)
    {
        return JHelper.RaycastAToB(_standing_pos + new Vector3(0, view_point.position.y),
            _position, settings.sight_distance, sight_test_layers);
    }


    // CollisionEventForwarder event.
    public void TriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("DamageableBody"))
            return;

        SquaddieAI squaddie = _other.GetComponentInParent<SquaddieAI>();

        if (squaddie == null || JHelper.SameFaction(squaddie, this))
        {
            return;
        }

        knowledge.nearby_targets.Add(squaddie);
    }


    // CollisionEventForwarder event.
    public void TriggerExit(Collider _other)
    {
        if (!_other.CompareTag("DamageableBody"))
            return;

        SquaddieAI squaddie = _other.GetComponentInParent<SquaddieAI>();

        if (squaddie == null)
            return;

        if (knowledge.nearby_targets.Contains(squaddie))
            knowledge.nearby_targets.Remove(squaddie);
    }


    // RaycastArray event.
    public void CoverForward(RaycastHit _hit)
    {
        cover_forward = HitValid(_hit);
    }


    // RaycastArray event.
    public void CoverBackward(RaycastHit _hit)
    {
        cover_backward = HitValid(_hit);
    }


    // RaycastArray event.
    public void CoverLeft(RaycastHit _hit)
    {
        cover_left = HitValid(_hit);
    }


    // RaycastArray event.
    public void CoverRight(RaycastHit _hit)
    {
        cover_right = HitValid(_hit);
    }


    bool HitValid(RaycastHit _hit)
    {
        return _hit.collider != null;
    }


    void Start()
    {
        original_height = mesh_transform.localPosition.y;
        original_leg_scale = leg_transform.localScale;

        scan_sphere.radius = settings.sight_distance;

        nav.isStopped = true;
        TransitionToState(current_state);

        InvokeRepeating("EvaluationTick", 0, settings.sense_delay);
    }


    /// <summary>
    /// A single update tick of the AI agent.
    /// Most of the AI's behaviour takes place inside the state system.
    /// The remainder of code handles animations or state-independent functionality.
    /// </summary>
    void Update()
    {
        current_state.UpdateState(this);

        knowledge.in_cover = cover_backward || cover_forward ||
                             cover_right || cover_left;

        HandleCrouch();
    }


    void OnEnable()
    {
        CancelInvoke("EvaluationTick");
        InvokeRepeating("EvaluationTick", 0, settings.sense_delay);
    }


    void OnDisable()
    {
        CancelInvoke("EvaluationTick");
        knowledge.chain_gun.cycle = false;
    }


    /// <summary>
    /// Animates the agent based on its crouched status, which is set through Actions.
    /// </summary>
    void HandleCrouch()
    {
        // Move mesh down when crouched as nav agent always stays at same height.
        Vector3 local_pos = mesh_transform.localPosition;
        local_pos.y = knowledge.crouched ?
            crouch_height : original_height;

        mesh_transform.localPosition = local_pos;

        // Also need to scale mesh so it doesn't clip through the floor.
        Vector3 leg_scale = knowledge.crouched ? new Vector3(original_leg_scale.x,
            crouch_height, original_leg_scale.z) : original_leg_scale;

        leg_transform.localScale = leg_scale;
    }


    /// <summary>
    /// Called when the AI transitions into a new state.
    /// </summary>
    /// <param name="_state">State that the AI is entering.</param>
    void OnStateEnter(State _state)
    {
        squaddie_canvas.UpdateStateDisplay(_state.display_text);
    }


    /// <summary>
    /// Called when the AI leaves its current state.
    /// </summary>
    /// <param name="_state">State that the AI is leaving.</param>
    void OnStateExit(State _state)
    {
        knowledge.state_time_elapsed = 0;
        knowledge.prev_state_time_elapsed = 0;
    }


    /// <summary>
    /// Refreshes the agent's combat awareness with up to date information.
    /// </summary>
    void EvaluationTick()
    {
        EvaluateClosestTarget();
        EvaluateCurrentTarget();

        if (knowledge.current_target != null)
        {
            EvaluateCurrentTargetVisibility();
            EvaluateCurrentTargetInRange();
            EvaluatePositionCompromised();
        }
        else
        {
            knowledge.current_target_visible = false;
            knowledge.current_target_in_range = false;
            knowledge.position_compromised = false;
        }
    }


    void EvaluateClosestTarget()
    {
        SquaddieAI closest_target = null;
        float closest = Mathf.Infinity;

        knowledge.nearby_targets.RemoveAll(elem => elem == null);
        foreach (SquaddieAI enemy in knowledge.nearby_targets)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance > closest)
                continue;

            closest = distance;
            closest_target = enemy;
        }

        knowledge.closest_target = closest_target;
    }


    /// <summary>
    /// Target Priority:
    /// 1. Order Target
    /// 2. Squad Target
    /// 3. Closest Target
    /// </summary>
    void EvaluateCurrentTarget()
    {
        knowledge.current_target = knowledge.closest_target;

        if (knowledge.order_target != null)
        {
            knowledge.current_target = knowledge.order_target;
        }
        else if (knowledge.squad_sense.squad_target != null &&
            knowledge.current_target == null)
        {
            knowledge.current_target = knowledge.squad_sense.squad_target;
        }

        // Inform squad of current target if the squad has no shared target.
        if (knowledge.current_target != null && knowledge.squad_sense.squad_target == null)
        {
            knowledge.squad_sense.squad_target = knowledge.current_target;
        }
    }


    void EvaluateCurrentTargetVisibility()
    {
        knowledge.current_target_visible = TestSightToPosition(
            knowledge.current_target.torso_transform.position);
    }


    void EvaluateCurrentTargetInRange()
    {
        float distance = Vector3.Distance(transform.position,
            knowledge.current_target.transform.position);

        bool too_close = distance < settings.minimum_engage_distance;
        bool too_far = distance > settings.maximum_engage_distance;

        knowledge.current_target_in_range = !too_close && !too_far;
    }


    void EvaluatePositionCompromised()
    {
        bool result = knowledge.current_target.TestSightToPosition(transform.position);
        knowledge.position_compromised = result;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = current_state.state_color;

        Gizmos.DrawSphere(nav.destination, 0.5f);
        Gizmos.DrawSphere(view_point.position + (view_point.forward * settings.minimum_engage_distance), 0.33f);
        Gizmos.DrawSphere(view_point.position + (view_point.forward * settings.maximum_engage_distance), 0.33f);

        Gizmos.DrawLine(view_point.position,
            view_point.position + (view_point.forward * settings.sight_distance));
    }


    void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Console"))
        {
            knowledge.nearby_console = _other.GetComponentInParent<HackableConsole>();
        }
    }


    void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Console"))
        {
            knowledge.nearby_console = null;
        }
    }

}
