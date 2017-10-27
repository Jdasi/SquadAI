using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;
using UnityEngine.AI;

public class SquaddieAI : MonoBehaviour
{
    [Header("Parameters")]
    public SquaddieSettings settings;
    [SerializeField] State current_state;
    [SerializeField] LayerMask sight_test_layers;
    [SerializeField] float crouch_height;

    [Header("Knowledge")]
    public WorkingKnowledge knowledge = new WorkingKnowledge();

    [Header("References")]
    public NavMeshAgent nav;
    public Transform view_point;
    public Transform torso_transform;
    public SquaddieStats stats;

    [Space]
    [SerializeField] Outline[] outlines;
    [SerializeField] MeshRenderer[] meshes;

    [Space]
    [SerializeField] Transform mesh_transform;
    [SerializeField] Transform leg_transform;
    [SerializeField] SquaddieCanvas squaddie_canvas;
    [SerializeField] SphereCollider scan_sphere;

    private float original_height;
    private Vector3 original_leg_scale;

    private bool cover_forward;
    private bool cover_backward;
    private bool cover_left;
    private bool cover_right;


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


    public void SetChainGun(ChainGun _gun)
    {
        knowledge.chain_gun = _gun;
    }


    public void LinkSquadSense(ref SquadSense _squad_sense)
    {
        knowledge.squad_sense = _squad_sense;
    }


    public void TransitionToState(State _target_state)
    {
        if (_target_state == null)
            return;

        OnStateExit(current_state);
        current_state = _target_state;
        OnStateEnter(current_state);
    }


    public void ResetOrderKnowledge()
    {
        knowledge.current_order = OrderType.NONE;
        knowledge.order_target = null;
        knowledge.order_waypoint = Vector3.zero;
        knowledge.order_console = null;

        nav.stoppingDistance = settings.move_stop_distance;
    }


    public void IssueMoveCommand(Vector3 _target)
    {
        knowledge.current_order = OrderType.MOVE;

        knowledge.order_target = null;
        knowledge.order_waypoint = _target;

        nav.destination = _target;
    }


    public void MoveToCoverNearPosition(Vector3 _position)
    {
        var cover_points = GameManager.scene.tactical_assessor.ClosestCoverPoints(
            _position, settings.cover_search_radius);

        if (cover_points.Count == 0)
            return;

        CoverPoint target_point = cover_points[0];
        nav.destination = target_point.position;
    }


    public void MoveToFlankEnemy(SquaddieAI _target, float _chase_range = -1)
    {
        _chase_range = _chase_range < 0 ? settings.chase_range : float.MaxValue;

        var cover_points = GameManager.scene.tactical_assessor.FindFlankingPositions(
            this, _target, settings.cover_search_radius);

        if (cover_points.Count == 0)
        {
            nav.destination = _target.transform.position;
        }
        else
        {
            foreach (CoverPoint cover_point in cover_points)
            {
                bool unique_pos = true;

                foreach (SquaddieAI ally in knowledge.squad_sense.squaddies)
                {
                    if (ally == this)
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

                nav.destination = cover_point.position;
                break;
            }
        }
    }


    public bool TestSightToPosition(Vector3 _position)
    {
        return JHelper.RaycastAToB(view_point.position, _position,
            settings.sight_distance, sight_test_layers);
    }


    public bool TestSightToPosition(Vector3 _standing_pos, Vector3 _position)
    {
        return JHelper.RaycastAToB(_standing_pos + new Vector3(0, view_point.position.y),
            _position, settings.sight_distance, sight_test_layers);
    }


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


    public void CoverForward(RaycastHit _hit)
    {
        cover_forward = HitValid(_hit);
    }


    public void CoverBackward(RaycastHit _hit)
    {
        cover_backward = HitValid(_hit);
    }


    public void CoverLeft(RaycastHit _hit)
    {
        cover_left = HitValid(_hit);
    }


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


    void OnStateEnter(State _state)
    {
        squaddie_canvas.UpdateStateDisplay(_state.display_text);
    }


    void OnStateExit(State _state)
    {
        knowledge.state_time_elapsed = 0;
        knowledge.prev_state_time_elapsed = 0;
    }


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


    void OnDrawGizmosSelected()
    {

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
