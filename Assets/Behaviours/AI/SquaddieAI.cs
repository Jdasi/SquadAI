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

public class SquaddieAI : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] bool engage_at_will = true;

    [Header("References")]
    [SerializeField] NavMeshAgent nav;
    [SerializeField] MeshRenderer body_mesh;
    [SerializeField] Transform view_point;
    [SerializeField] SquaddieStats stats;
    [SerializeField] SquaddieCanvas squaddie_canvas;

    private SquaddieState state = SquaddieState.IDLE;
    private List<SquaddieAI> squad_members; // Reference.
    private List<Transform> nearby_targets = new List<Transform>();
    private ChainGun chain_gun;


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
        chain_gun = _gun;
    }


    public void ChangeMaterial(Material _material)
    {
        body_mesh.material = _material;
    }


    public void LinkSquaddieList(ref List<SquaddieAI> _squaddies)
    {
        squad_members = _squaddies;
    }


    public void IssueWaypoint(Vector3 _target)
    {
        nav.destination = _target;
        state = SquaddieState.MOVING;
    }


    public void TriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("DamageableBody"))
            return;

        SquaddieStats squaddie = _other.GetComponentInParent<SquaddieStats>();

        if (squaddie == null || JHelper.SameFaction(squaddie.faction_settings,
            stats.faction_settings))
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
        if (nearby_targets.Count > 0)
        {
            state = SquaddieState.ENGAGING;
        }
    }


    void MovingState()
    {
        if (nav.isStopped && nav.hasPath)
            nav.isStopped = false;

        if (nav.hasPath && nav.remainingDistance <= nav.stoppingDistance)
        {
            state = SquaddieState.IDLE;
            nav.isStopped = true;
        }

        if (engage_at_will && nearby_targets.Count > 0)
        {
            state = SquaddieState.ENGAGING;
        }
    }


    void TakingCoverState()
    {

    }


    void EngagingState()
    {
        nav.isStopped = true;

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
            bool hit_success = Physics.Raycast(view_point.position, transform.forward, out hit, Mathf.Infinity,
                1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Damageable"));

            if (hit_success)
            {
                target_hit = hit.transform == closest_target;
            }

        }

        chain_gun.cycle = target_hit;

        if (nearby_targets.Count == 0)
        {
            state = SquaddieState.IDLE;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(view_point.position,
            transform.position + (transform.forward * 100));
    }

}
