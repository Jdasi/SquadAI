using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TacticalAssessor : MonoBehaviour
{
    [SerializeField] LayerMask blocking_layers;


    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, float _radius)
    {
        var cover_points = FindCoverPoints(_position, _radius);

        foreach (CoverPoint cover_point in cover_points)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(cover_point.position, _position, NavMesh.AllAreas, path);

            for (int i = 0; i < path.corners.Length; ++i)
            {
                if (i + 1 >= path.corners.Length)
                    break;

                float distance = Vector3.Distance(path.corners[i], path.corners[i + 1]);
                cover_point.weighting += distance;
            }
        }

        cover_points = cover_points.OrderBy(elem => elem.weighting).ToList();
        return cover_points;
    }


    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, Vector3 _required_normal, float _radius)
    {
        var cover_points = ClosestCoverPoints(_position, _radius);
        cover_points.RemoveAll(elem => elem.normal != _required_normal);

        return cover_points;
    }


    public List<CoverPoint> FindFlankingPositions(SquaddieAI _flanker, SquaddieAI _flank_target, float _radius)
    {
        // Consider nearby cover points, as well as those near the enemy.
        var cover_to_consider = ClosestCoverPoints(_flanker.transform.position, _radius);
        cover_to_consider.AddRange(ClosestCoverPoints(_flank_target.transform.position, _radius));

        // The actual points that will be returned.
        List<CoverPoint> flanking_positions = new List<CoverPoint>();

        foreach (CoverPoint cover_point in cover_to_consider)
        {
            float distance = Vector3.Distance(_flank_target.transform.position, cover_point.position);

            bool too_far = distance > _flanker.settings.maximum_engage_distance;
            bool too_close = distance < _flanker.settings.minimum_engage_distance;

            if (too_far || too_close)
                continue;

            bool bad_flank = !_flanker.TestSightToPosition(cover_point.position,
                _flank_target.torso_transform.position);
            bool enemy_los = _flank_target.TestSightToPosition(cover_point.position);

            if (bad_flank || enemy_los)
                continue;

            // The further from the flank target the better.
            cover_point.weighting -= distance;

            // Encourage squad cohesion.
            cover_point.weighting += (cover_point.position -
                _flanker.knowledge.squad_sense.squad_center).magnitude;

            flanking_positions.Add(cover_point);
        }

        flanking_positions = flanking_positions.OrderBy(elem => elem.weighting).ToList();
        return flanking_positions;
    }


    List<CoverPoint> FindCoverPoints(Vector3 _position, float _radius)
    {
        List<CoverPoint> cover_points = new List<CoverPoint>();

        foreach (CoverPoint cover_point in GameManager.scene.cover_point_generator.cover_points)
        {
            cover_point.weighting = 0;

            if (cover_point.occupied)
                continue;

            float distance = Vector3.Distance(_position, cover_point.position);

            if (distance > _radius)
                continue;

            cover_points.Add(cover_point);
        }

        return cover_points;
    }

}
