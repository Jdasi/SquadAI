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


    public List<CoverPoint> FlankingPositions(SquaddieAI _flanker, SquaddieAI _flank_target, float _radius)
    {
        var cover_points = ClosestCoverPoints(_flank_target.transform.position, _radius);

        foreach (CoverPoint cover_point in cover_points)
        {
            float distance = Vector3.Distance(_flank_target.view_point.position, cover_point.position);
            Vector3 direction = (cover_point.position - _flank_target.view_point.position).normalized;

            if (distance > _flanker.settings.maximum_engage_distance)
                cover_point.weighting += 1000;

            if (EnemyHasLOS(cover_point, _flanker, _flank_target, direction, distance))
            {
                cover_point.weighting += 100;
            }
            else if (GoodFlankSpot(cover_point, _flanker, _flank_target, distance))
            {
                cover_point.weighting += -25;
            }
        }

        cover_points = cover_points.OrderBy(elem => elem.weighting).ToList();
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


    bool EnemyHasLOS(CoverPoint _cover_point, SquaddieAI _squaddie, SquaddieAI _enemy,
        Vector3 _direction, float _distance)
    {
        RaycastHit enemy_ray;
        bool enemy_ray_blocked = Physics.Raycast(_enemy.view_point.position, _direction,
            out enemy_ray, _distance, blocking_layers);

        if (enemy_ray_blocked)
            return false;

        return true;
    }


    bool GoodFlankSpot(CoverPoint _cover_point, SquaddieAI _squaddie, SquaddieAI _enemy,
        float _distance)
    {
        Vector3 view_pos = _cover_point.position;
        view_pos.y = _squaddie.view_point.position.y;

        Vector3 flank_dir = (_enemy.view_point.position - view_pos).normalized;

        RaycastHit squaddie_ray;
        bool squaddie_ray_blocked = Physics.Raycast(view_pos, flank_dir, out squaddie_ray,
            _distance, blocking_layers);

        if (squaddie_ray_blocked)
            return false;

        return true;
    }

}
