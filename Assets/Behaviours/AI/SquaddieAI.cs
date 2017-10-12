﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquaddieAI : MonoBehaviour
{
    [Header("Parameters")]
    public State current_state;

    [Header("Knowledge")]
    public WorkingKnowledge knowledge = new WorkingKnowledge();

    [Header("References")]
    public NavMeshAgent nav;
    public MeshRenderer body_mesh;
    public Transform view_point;
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


    public void IssueWaypoint(Vector3 _target)
    {
        knowledge.has_order = true;
        knowledge.order_waypoint = _target;

        nav.destination = _target;
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
    }


    void Update()
    {
        knowledge.nearby_targets.RemoveAll(elem => elem == null);
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


    void OnDrawGizmosSelected()
    {
        Gizmos.color = current_state.state_color;
        Gizmos.DrawLine(view_point.position,
            view_point.position + (view_point.forward * 100));
    }

}
