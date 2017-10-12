using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu (menuName = "Custom/Weighting Modules/Distance")]
public class DistanceWeight : WeightingModule
{
    [SerializeField] float adjustment_per_unit;


    public override void AdjustWeight(CoverPoint _cover_point, CurrentContext _context, SquaddieAI _squaddie)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(_cover_point.position, _context.indicator_position, NavMesh.AllAreas, path);

        for (int i = 0; i < path.corners.Length; ++i)
        {
            if (i + 1 >= path.corners.Length)
                break;

            float distance = Vector3.Distance(path.corners[i], path.corners[i + 1]);
            _cover_point.weighting += distance * adjustment_per_unit;
        }
    }

}
