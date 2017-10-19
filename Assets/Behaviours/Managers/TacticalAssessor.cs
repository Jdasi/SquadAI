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
        var cover_points = ClosestCoverPoints(_flank_target.transform.position, _radius);

        for (int i = cover_points.Count - 1; i > 0; --i)
        {
            CoverPoint cover_point = cover_points[i];
            float distance = Vector3.Distance(_flank_target.transform.position, cover_point.position);

            bool too_far = distance > _flanker.settings.maximum_engage_distance;
            bool too_close = distance < _flanker.settings.minimum_engage_distance;
            bool bad_flank = !_flanker.TestSightToPosition(cover_point.position, _flank_target.torso_transform.position);
            bool enemy_los = _flank_target.TestSightToPosition(cover_point.position);

            if (too_far || too_close || bad_flank || enemy_los)
            {
                cover_points.Remove(cover_point);
            }
        }

        cover_points = cover_points.OrderByDescending(elem => elem.weighting).ToList();
        return cover_points;
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
