using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Communicates with the CoverPointGenerator to assess the tactical viability of 
/// cover points, based on a number of parameters.
/// Other classes can query this one for advantageous positions in combat.
/// </summary>
public class TacticalAssessor : MonoBehaviour
{

    /// <summary>
    /// Evaluates the closest cover points to the passed position inside the passed radius.
    /// </summary>
    /// <param name="_position">The origin to search for cover points.</param>
    /// <param name="_radius">The radius to search from the location.</param>
    /// <returns>A list of cover points in ascending order based on their proximity to the passed position.</returns>
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

        // Positions closest to the passed position should appear first in the list.
        cover_points = cover_points.OrderBy(elem => elem.weighting).ToList();
        return cover_points;
    }


    /// <summary>
    /// Evaluates the closest cover points to the passed position inside the passed radius.
    /// </summary>
    /// <param name="_position">The origin to search for cover points.</param>
    /// <param name="_required_normal">The required normal stored by the cover point.</param>
    /// <param name="_radius">The radius to search from the location.</param>
    /// <returns>A list of cover points in ascending order based on their proximity to the passed position.</returns>
    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, Vector3 _required_normal, float _radius)
    {
        var cover_points = ClosestCoverPoints(_position, _radius);
        cover_points.RemoveAll(elem => elem.normal != _required_normal);

        return cover_points;
    }


    /// <summary>
    /// Evaluates advantageous engagement positions from the flanker to the flank target, within
    /// the passed radius.
    /// </summary>
    /// <param name="_flanker">The flanking squaddie.</param>
    /// <param name="_flank_target">The target of the flank.</param>
    /// <param name="_radius">The radius to search for cover points around the flanker and flank target.</param>
    /// <returns>A list of cover points in descending order based on their distance to the flank target.</returns>
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

        // The furthest flanking positions should appear first in the list.
        flanking_positions = flanking_positions.OrderBy(elem => elem.weighting).ToList();
        return flanking_positions;
    }


    /// <summary>
    /// Evaluates the cover points that are within the passed radius of the passed position.
    /// </summary>
    /// <param name="_position">The origin to search for cover points.</param>
    /// <param name="_radius">The radius to search from the location.</param>
    /// <returns>A list of cover points.</returns>
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
