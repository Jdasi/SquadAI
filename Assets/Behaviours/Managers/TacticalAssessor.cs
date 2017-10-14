using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TacticalAssessor : MonoBehaviour
{
    [SerializeField] WeightingModule[] weighting_modules;


    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, float _radius)
    {
        var cover_points = FindCoverPoints(_position, _radius);

        foreach (CoverPoint cover_point in cover_points)
        {
            float distance = Vector3.Distance(_position, cover_point.position);
            cover_point.weighting = distance;
        }

        cover_points = cover_points.OrderByDescending(elem => elem.weighting).ToList();
        return cover_points;
    }


    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, Vector3 _required_normal, float _radius)
    {
        var cover_points = ClosestCoverPoints(_position, _radius);
        cover_points.RemoveAll(elem => elem.normal != _required_normal);

        return cover_points;
    }


    List<CoverPoint> FindCoverPoints(Vector3 _position, float _radius)
    {
        List<CoverPoint> cover_points = new List<CoverPoint>();

        foreach (CoverPoint cover_point in GameManager.scene.cover_point_generator.cover_points)
        {
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
