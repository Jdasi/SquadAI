using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquaddieAI : MonoBehaviour
{
    [Header("Parameters")]
    public State current_state;
    public SquaddieSettings settings;
    [SerializeField] LayerMask sight_blocking_layers;
    [SerializeField] LayerMask sight_test_layers;
    [SerializeField] float evaluation_interval = 0.1f;
    [SerializeField] float crouch_height;

    [Header("Knowledge")]
    public WorkingKnowledge knowledge = new WorkingKnowledge();

    [Header("References")]
    public NavMeshAgent nav;
    public MeshRenderer[] meshes;
    public Transform view_point;
    public Transform mesh_transform;
    public Transform torso_transform;
    public Transform leg_transform;
    public SquaddieStats stats;
    public SquaddieCanvas squaddie_canvas;

    private float original_height;
    private Vector3 original_leg_scale;

    private bool cover_forward;
    private bool cover_backward;
    private bool cover_left;
    private bool cover_right;


    public void Init()
    {
        if (stats.faction_settings == null)
            return;

        squaddie_canvas.Init(stats.faction_settings);
        SetSelected(false);
    }


    public void SetSelected(bool _selected)
    {
        Material material = _selected ? stats.faction_settings.select_material :
            stats.faction_settings.deselect_material;
        
        foreach (MeshRenderer mesh in meshes)
            mesh.material = material;
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

        if (cover_points.Count <= 0)
            return;

        CoverPoint target_point = cover_points[0];
        nav.destination = target_point.position;
    }


    public void MoveToFlankEnemy(SquaddieAI _target)
    {
        var cover_points = GameManager.scene.tactical_assessor.FlankingPositions(
            this, _target, settings.cover_search_radius);

        if (cover_points.Count <= 0)
            return;

        CoverPoint target_point = cover_points[0];
        nav.destination = target_point.position;
    }


    public bool TestSightToPosition(Vector3 _position)
    {
        Vector3 dir = (_position - view_point.position).normalized;
        float dist = Vector3.Distance(_position, view_point.position);

        RaycastHit hit;
        bool ray_blocked = Physics.Raycast(view_point.position, dir, out hit,
            settings.sight_distance, sight_test_layers);

        if (ray_blocked && hit.distance > dist)
            return true;

        return !ray_blocked && dist <= settings.sight_distance;
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

        nav.isStopped = true;
        TransitionToState(current_state);

        InvokeRepeating("EvaluationTick", 0, evaluation_interval);
    }


    void Update()
    {
        current_state.UpdateState(this);

        knowledge.in_cover = cover_backward || cover_forward ||
                             cover_right || cover_left;

        HandleCrouch();
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
        EvaluateSightHit();
        EvaluateClosestTarget();
        EvaluateCurrentTarget();
        EvaluateCurrentTargetVisibility();
    }


    void EvaluateSightHit()
    {
        Physics.Raycast(view_point.position, view_point.forward, out knowledge.sight_hit,
            settings.sight_distance, sight_blocking_layers);
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
            float distance = Vector3.Distance(knowledge.order_target.transform.position,
                transform.position);

            if (distance < settings.maximum_engage_distance ||
                knowledge.closest_target == null)
            {
                knowledge.current_target = knowledge.order_target;
            }
        }
    }


    void EvaluateCurrentTargetVisibility()
    {
        if (knowledge.current_target == null)
        {
            knowledge.current_target_visible = false;
            return;
        }

        knowledge.current_target_visible = TestSightToPosition(
            knowledge.current_target.torso_transform.position);
    }


    void OnDisable()
    {
        knowledge.chain_gun.cycle = false;
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

}
