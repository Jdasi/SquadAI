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

    [Header("Knowledge")]
    public WorkingKnowledge knowledge = new WorkingKnowledge();

    [Header("References")]
    public NavMeshAgent nav;
    public MeshRenderer body_mesh;
    public Transform view_point;
    public Transform collider_transform;
    public SquaddieStats stats;
    public SquaddieCanvas squaddie_canvas;


    public void Init()
    {
        if (stats.faction_settings == null)
            return;

        squaddie_canvas.Init(stats.faction_settings);
        SetSelected(false);
    }


    public void SetSelected(bool _selected)
    {
        body_mesh.material = _selected ? stats.faction_settings.select_material :
            stats.faction_settings.deselect_material;
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
        knowledge.has_order = true;
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


    public bool TestSightToPosition(Vector3 _position)
    {
        Vector3 dir = (_position - view_point.position).normalized;
        float dist = Vector3.Distance(_position, view_point.position);

        RaycastHit hit;
        bool ray_blocked = Physics.Raycast(view_point.position, dir, out hit,
            settings.sight_distance, sight_test_layers);

        if (ray_blocked && hit.distance > dist)
            return true;

        return !ray_blocked;
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


    void Start()
    {
        nav.isStopped = true;
        TransitionToState(current_state);

        InvokeRepeating("EvaluationTick", 0, evaluation_interval);
    }


    void Update()
    {
        current_state.UpdateState(this);
    }


    void OnStateEnter(State _state)
    {
        squaddie_canvas.UpdateStateDisplay(_state.display_text);
    }


    void OnStateExit(State _state)
    {
        knowledge.state_time_elapsed = 0;
    }


    void EvaluationTick()
    {
        EvaluateSightHit();
        EvaluateClosestTarget();
        EvaluateClosestTargetInSight();
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


    void EvaluateClosestTargetInSight()
    {
        if (knowledge.closest_target == null)
            return;

        knowledge.closest_target_visible = TestSightToPosition(
            knowledge.closest_target.collider_transform.position);
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
