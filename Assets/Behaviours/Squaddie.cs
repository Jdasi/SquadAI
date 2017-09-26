using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Squaddie : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NavMeshAgent agent;


    public void IssueWaypoint(Vector3 _waypoint)
    {
        agent.destination = _waypoint;
    }


}
