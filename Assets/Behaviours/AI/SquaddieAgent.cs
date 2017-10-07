using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.AI;

public enum SquaddieState
{
    IDLE,
    MOVING,
    TAKING_COVER,
    ENGAGING
}

public class SquaddieAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] SquaddieStats squaddie_stats;
    [SerializeField] SquaddieCanvas squaddie_canvas;

    private SquaddieState state = SquaddieState.IDLE;
    private List<SquaddieAgent> squad_members; // Reference.
    private List<Transform> nearby_targets = new List<Transform>();
    private ChainGun chain_gun;


    public void Init()
    {
        if (squaddie_stats.faction_settings == null)
            return;

        squaddie_canvas.Init(squaddie_stats.faction_settings);
        Deselect();
    }


    public void Select()
    {
        mesh.material = squaddie_stats.faction_settings.select_material;
    }


    public void Deselect()
    {
        mesh.material = squaddie_stats.faction_settings.deselect_material;
    }


    public void SetChainGun(ChainGun _gun)
    {
        chain_gun = _gun;
    }


    public void ChangeMaterial(Material _material)
    {
        mesh.material = _material;
    }


    public void LinkSquaddieList(ref List<SquaddieAgent> _squaddies)
    {
        squad_members = _squaddies;
    }


    public void IssueWaypoint(Vector3 _target)
    {
        agent.destination = _target;
        state = SquaddieState.MOVING;
    }


    public void TriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("DamageableBody"))
            return;

        SquaddieStats squaddie = _other.GetComponentInParent<SquaddieStats>();

        if (squaddie == null || JHelper.SameFaction(squaddie.faction_settings,
            squaddie_stats.faction_settings))
        {
            return;
        }

        nearby_targets.Add(squaddie.transform);
    }


    public void TriggerExit(Collider _other)
    {
        if (!_other.CompareTag("DamageableBody"))
            return;

        if (nearby_targets.Contains(_other.transform.parent))
            nearby_targets.Remove(_other.transform.parent);
    }


    void Update()
    {
        nearby_targets.RemoveAll(elem => elem == null);

        switch (state)
        {
            case SquaddieState.IDLE:         IdleState();        break;
            case SquaddieState.MOVING:       MovingState();      break;
            case SquaddieState.TAKING_COVER: TakingCoverState(); break;
            case SquaddieState.ENGAGING:     EngagingState();    break;
        }

        squaddie_canvas.UpdateStateDisplay(state);
    }


    void IdleState()
    {
        Transform closest_target = null;
        float closest_distance = Mathf.Infinity;

        foreach (Transform target in nearby_targets)
        {
            float distance = (target.position - transform.position).sqrMagnitude;
            if (distance >= closest_distance)
                continue;

            closest_target = target;
            closest_distance = distance;
        }

        bool target_hit = false;

        if (closest_target != null)
        {
            transform.LookAt(closest_target);

            RaycastHit hit;
            bool hit_success = Physics.Raycast(transform.position + new Vector3(0, 1), transform.forward, out hit, Mathf.Infinity,
                1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Damageable"));

            if (hit_success)
            {
                target_hit = hit.transform == closest_target;
            }

        }

        chain_gun.cycle = target_hit;
    }


    void MovingState()
    {
        if (agent.isStopped && agent.hasPath)
            agent.isStopped = false;

        if (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            state = SquaddieState.IDLE;
            agent.isStopped = true;
        }
    }


    void TakingCoverState()
    {

    }


    void EngagingState()
    {

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0, 1),
            transform.position + (transform.forward * 100));
    }

}
