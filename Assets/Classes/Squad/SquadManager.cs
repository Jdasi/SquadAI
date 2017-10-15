using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SquadManager
{
    public int num_squaddies { get { return squad_sense.squaddies.Count; } }
    public SquadSettings settings;

    private SquadSense squad_sense = new SquadSense();
    private SquadBlock ui_block;


    public SquadManager(SquadSettings _settings, ref SquadBlock _ui_block)
    {
        settings = _settings;
        ui_block = _ui_block;
    }


    public void SelectSquad()
    {
        foreach (SquaddieAI squaddie in squad_sense.squaddies)
            squaddie.SetSelected(true);

        ui_block.Select();
    }


    public void DeselectSquad()
    {
        foreach (SquaddieAI squaddie in squad_sense.squaddies)
            squaddie.SetSelected(false);

        ui_block.Deselect();
    }


    public void AddSquaddie(SquaddieAI _squaddie_ai)
    {
        if (squad_sense.squaddies.Contains(_squaddie_ai))
            return;

        squad_sense.squaddies.Add(_squaddie_ai);

        _squaddie_ai.LinkSquadSense(ref squad_sense);
        _squaddie_ai.SetSelected(true);

        ui_block.UpdateUnitCount(squad_sense.squaddies.Count);
    }


    public void RemoveSquaddie()
    {
        if (num_squaddies == 0)
            return;

        Object.Destroy(squad_sense.squaddies[squad_sense.squaddies.Count - 1].gameObject);
    }


    public void IssueContextCommand(CurrentContext _context)
    {
        switch (_context.type)
        {
            case ContextType.FLOOR:
            {
                SquadMoveCommand(_context);
            } break;

            case ContextType.COVER:
            {
                SquadCoverCommand(_context);
            } break;

            case ContextType.ATTACK:
            {
                SquadAttackCommand(_context);
            } break;
        }
    }


    public void IssueFollowCommand()
    {
        squad_sense.follow_targets.Clear();
        squad_sense.follow_targets.Add(GameManager.scene.player.transform);

        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            squad_sense.follow_targets.Add(squaddie.transform);

            squaddie.nav.stoppingDistance = squaddie.settings.follow_stop_distance;
            squaddie.knowledge.current_order = OrderType.FOLLOW;
        }

        for (int i = 0; i < squad_sense.squaddies.Count; ++i)
        {
            squad_sense.squaddies[i].knowledge.follow_target = squad_sense.follow_targets[i];
        }

        squad_sense.follow_targets.Clear();
    }


    public void Update()
    {
        GarbageCollect();
        EvaluateSquadCenter();
    }


    void GarbageCollect()
    {
        int prev_count = squad_sense.squaddies.Count;
        squad_sense.squaddies.RemoveAll(elem => elem == null);

        if (squad_sense.squaddies.Count != prev_count)
            ui_block.UpdateUnitCount(squad_sense.squaddies.Count);
    }


    void EvaluateSquadCenter()
    {
        Vector3 avg_pos = Vector3.zero;

        foreach (SquaddieAI squaddie in squad_sense.squaddies)
            avg_pos += squaddie.transform.position;

        avg_pos /= squad_sense.squaddies.Count;
        squad_sense.squad_center = avg_pos;
    }


    void SquadMoveCommand(CurrentContext _context)
    {
        List<float> squaddie_sizes = new List<float>();
        float line_width = 0;

        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            squaddie.nav.stoppingDistance = squaddie.settings.move_stop_distance;

            line_width += squaddie.nav.radius + settings.squaddie_spacing;
            squaddie_sizes.Add(squaddie.nav.radius);
        }

        for (int i = 0; i < num_squaddies; ++i)
        {
            float padded_squaddie = squad_sense.squaddies[i].nav.radius + settings.squaddie_spacing;

            Vector3 waypoint = _context.indicator_position + (_context.indicator_right * (padded_squaddie * i));
            waypoint -= _context.indicator_right * ((line_width - padded_squaddie) / 2);

            squad_sense.squaddies[i].IssueMoveCommand(waypoint);
        }
    }


    void SquadCoverCommand(CurrentContext _context)
    {
        List<CoverPoint> allocated_points = new List<CoverPoint>();

        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            squaddie.nav.stoppingDistance = squaddie.settings.move_stop_distance;

            var cover_points = GameManager.scene.tactical_assessor.ClosestCoverPoints(
                _context.indicator_position, squaddie.settings.cover_search_radius);

            if (cover_points.Count <= 0)
                break;

            CoverPoint target_point = null;
            foreach (CoverPoint cover_point in cover_points)
            {
                if (target_point != null)
                    break;

                if (allocated_points.Contains(cover_point))
                    continue;

                target_point = cover_point;
            }

            if (target_point == null)
                break;

            allocated_points.Add(target_point);
            squaddie.IssueMoveCommand(target_point.position);
        }
    }


    void SquadAttackCommand(CurrentContext _context)
    {
        SquaddieAI target = _context.indicator_hit.GetComponent<SquaddieAI>();
        
        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            if (JHelper.SameFaction(squaddie, target))
                continue;

            squaddie.knowledge.order_target = target;
            squaddie.knowledge.current_order = OrderType.ATTACK;
        }
    }

}
