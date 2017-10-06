using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Squaddie : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] MeshRenderer mesh;

    private List<Squaddie> squad_members;


    public void ChangeMaterial(Material _material)
    {
        mesh.material = _material;
    }


    public void LinkSquaddieList(ref List<Squaddie> _squaddies)
    {
        squad_members = _squaddies;
    }


    public void IssueWaypoint(Vector3 _target)
    {
        agent.destination = _target;
    }


    public void Update()
    {

    }

}
