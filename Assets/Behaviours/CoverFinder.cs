using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverFinder : MonoBehaviour
{
    [SerializeField] SphereCollider sphere;


    void Update()
    {
        
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        NavMeshHit hit;
        
        if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            if (hit.distance > sphere.radius)
                return;

            Gizmos.DrawSphere(hit.position, 0.25f);
        }
    }

}
