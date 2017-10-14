using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquaddieAI : MonoBehaviour
{
    [Header("Parameters")]
    public State current_state;
    public SquaddieSettings settings;

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
        Deselect();
    }


    public void Select()
    {
        body_mesh.material = stats.faction_settings.select_material;
    }


    public void Deselect()
    {
        body_mesh.material = stats.faction_settings.deselect_material;
    }


    public void SetChainGun(ChainGun _gun)
    {
        knowledge.chain_gun = _gun;
    }


    public void ChangeMaterial(Material _material)
    {
        body_mesh.material = _material;
    }


    public void LinkSquaddieList(ref List<SquaddieAI> _squaddies)
    {
        knowledge.squad_members = _squaddies;
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


    void EvaluateClosestTarget()
    {
        SquaddieAI target = null;
        float closest = Mathf.Infinity;

        foreach (SquaddieAI enemy in knowledge.nearby_targets)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance > closest)
                continue;

            closest = distance;
            target = enemy;
        }

        knowledge.closest_target = target;
    }


    void Start()
    {
        nav.isStopped = true;
        TransitionToState(current_state);
    }


    void Update()
    {
        knowledge.nearby_targets.RemoveAll(elem => elem == null);
        current_state.UpdateState(this);

        EvaluateClosestTarget();
    }


    void OnStateEnter(State _state)
    {
        squaddie_canvas.UpdateStateDisplay(_state.display_text);
    }


    void OnStateExit(State _state)
    {
        knowledge.state_time_elapsed = 0;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = current_state.state_color;
        Gizmos.DrawSphere(nav.destination, 0.5f);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = current_state.state_color;
        Gizmos.DrawLine(view_point.position,
            view_point.position + (view_point.forward * 100));
    }

}
