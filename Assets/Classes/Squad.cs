using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad
{
    public int num_squaddies { get { return squaddies.Count; } }
    public SquadSettings settings;

    private List<Squaddie> squaddies = new List<Squaddie>();


    public Squad(SquadSettings _settings)
    {
        settings = _settings;
    }


    public void Update()
    {
        squaddies.RemoveAll(elem => elem == null);
    }


    public void AddSquaddie(Squaddie _squaddie)
    {
        if (squaddies.Contains(_squaddie))
            return;

        squaddies.Add(_squaddie);
    }


    public void RemoveSquaddie()
    {
        if (num_squaddies == 0)
            return;

        Object.Destroy(squaddies[squaddies.Count - 1].gameObject);
        squaddies.Remove(squaddies[squaddies.Count - 1]);
    }


    public void IssueContextCommand(CurrentContext _context)
    {
        switch (_context.type)
        {
            case ContextType.FLOOR:
            {
                IssueMoveCommand(_context);
            } break;

            case ContextType.COVER:
            {
                IssueCoverCommand(_context);
            } break;
        }
    }


    void IssueMoveCommand(CurrentContext _context)
    {
        float padded_squaddie = settings.squaddie_size + settings.squaddie_spacing;
        float line_width = padded_squaddie * num_squaddies;

        for (int i = 0; i < num_squaddies; ++i)
        {
            Vector3 waypoint = _context.indicator_position + (_context.indicator_right * (padded_squaddie * i));
            waypoint -= _context.indicator_right * ((line_width - padded_squaddie) / 2);

            squaddies[i].IssueWaypoint(waypoint);
        }
    }


    void IssueCoverCommand(CurrentContext _context)
    {
        var cover_points = GameManager.scene.cover_point_generator.ClosestCoverPoints(
            _context.indicator_position, _context.indicator_normal, settings.cover_search_radius);

        foreach (Squaddie squaddie in squaddies)
        {
            if (cover_points.Count <= 0)
                break;

            CoverPoint cover_point = cover_points[Random.Range(0, cover_points.Count)];
            cover_points.Remove(cover_point);

            squaddie.IssueWaypoint(cover_point.position);
        }
    }

}
