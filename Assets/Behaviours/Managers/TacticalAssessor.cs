using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TacticalAssessor : MonoBehaviour
{
    [SerializeField] WeightingModule[] weighting_modules;


    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, float _distance,
        SquaddieAI _squaddie)
    {
        var cover_points = GameManager.scene.cover_point_generator.cover_points;
        List<CoverPoint> closest_cover_points = new List<CoverPoint>();

        foreach (CoverPoint cover_point in cover_points)
        {
            if (cover_point.occupied || Vector3.Distance(_position, cover_point.position) > _distance)
                continue;

            // Reset weighting and adjust for squaddie.
            cover_point.weighting = 0;
            foreach (WeightingModule weighting_module in weighting_modules)
                weighting_module.AdjustWeight(cover_point, GameManager.scene.context_scanner.current_context, _squaddie);

            closest_cover_points.Add(cover_point);
        }

        closest_cover_points = closest_cover_points.OrderByDescending(elem => elem.weighting).ToList();
        return closest_cover_points;
    }


    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, Vector3 _required_normal, float _distance,
        SquaddieAI _squaddie)
    {
        var cover_points = GameManager.scene.cover_point_generator.cover_points;
        List<CoverPoint> closest_cover_points = new List<CoverPoint>();

        foreach (CoverPoint cover_point in cover_points)
        {
            if (cover_point.occupied || cover_point.normal != _required_normal)
                continue;

            // Reset weighting and adjust for squaddie.
            cover_point.weighting = 0;
            foreach (WeightingModule weighting_module in weighting_modules)
                weighting_module.AdjustWeight(cover_point, GameManager.scene.context_scanner.current_context, _squaddie);

            if (Vector3.Distance(_position, cover_point.position) <= _distance)
                closest_cover_points.Add(cover_point);
        }

        closest_cover_points = closest_cover_points.OrderByDescending(elem => elem.weighting).ToList();
        return closest_cover_points;
    }

}
