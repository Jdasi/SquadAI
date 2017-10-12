using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager
{
    public int num_squaddies { get { return squaddies.Count; } }
    public SquadSettings settings;

    private List<SquaddieAI> squaddies = new List<SquaddieAI>();
    private SquadBlock ui_block;


    public SquadManager(SquadSettings _settings, ref SquadBlock _ui_block)
    {
        settings = _settings;
        ui_block = _ui_block;
    }


    public void SelectSquad()
    {
        foreach (SquaddieAI squaddie in squaddies)
            squaddie.Select();

        ui_block.Select();
    }


    public void DeselectSquad()
    {
        foreach (SquaddieAI squaddie in squaddies)
            squaddie.Deselect();

        ui_block.Deselect();
    }


    public void AddSquaddie(SquaddieAI _squaddie_ai)
    {
        if (squaddies.Contains(_squaddie_ai))
            return;

        squaddies.Add(_squaddie_ai);

        _squaddie_ai.LinkSquaddieList(ref squaddies);
        _squaddie_ai.Select();

        ui_block.UpdateUnitCount(squaddies.Count);
    }


    public void RemoveSquaddie()
    {
        if (num_squaddies == 0)
            return;

        Object.Destroy(squaddies[squaddies.Count - 1].gameObject);
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


    public void Update()
    {
        GarbageCollect();
    }


    void GarbageCollect()
    {
        int prev_count = squaddies.Count;
        squaddies.RemoveAll(elem => elem == null);

        if (squaddies.Count != prev_count)
            ui_block.UpdateUnitCount(squaddies.Count);
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
        List<CoverPoint> allocated_points = new List<CoverPoint>();

        foreach (SquaddieAI squaddie in squaddies)
        {
            var cover_points = GameManager.scene.tactical_assessor.ClosestCoverPoints(
                _context.indicator_position, settings.cover_search_radius, squaddie);

            if (cover_points.Count <= 0)
                break;

            CoverPoint target_point = null;
            foreach (CoverPoint cover_point in cover_points)
            {
                if (allocated_points.Contains(cover_point))
                    continue;

                target_point = cover_point;
            }

            if (target_point == null)
                break;

            allocated_points.Add(target_point);
            squaddie.IssueWaypoint(target_point.position);
        }
    }

}
