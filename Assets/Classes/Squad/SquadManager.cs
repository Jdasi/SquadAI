using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SquadManager
{
    public bool selected { get; private set; }
    public SquadSense squad_sense = new SquadSense();
    public FactionSettings squad_faction { get { return squad_sense.faction; } }
    public int num_squaddies { get { return squad_sense.squaddies.Count; } }
    public TargetBobber order_target_bobber { get; private set; }

    private const float SQUADDIE_SPACING = 1;


    public SquadManager(FactionSettings _faction)
    {
        squad_sense.faction = _faction;
    }


    public void AssignTargetBobber(GameObject _bobber)
    {
        order_target_bobber = _bobber.GetComponent<TargetBobber>();
    }


    public void SelectSquad()
    {
        foreach (SquaddieAI squaddie in squad_sense.squaddies)
            squaddie.SetSelected(true);

        selected = true;
    }


    public void DeselectSquad()
    {
        foreach (SquaddieAI squaddie in squad_sense.squaddies)
            squaddie.SetSelected(false);

        selected = false;
    }


    public void AddSquaddie(SquaddieAI _squaddie_ai)
    {
        if (squad_sense.squaddies.Contains(_squaddie_ai))
            return;

        _squaddie_ai.Init(squad_faction);
        _squaddie_ai.LinkSquadSense(ref squad_sense);

        squad_sense.squaddies.Add(_squaddie_ai);
    }


    public void RemoveSquaddie()
    {
        if (num_squaddies == 0)
            return;

        Object.Destroy(squad_sense.squaddies[squad_sense.squaddies.Count - 1].gameObject);
    }


    public void IssueContextCommand()
    {
        squad_sense.squad_target = null;
        CurrentContext context = GameManager.scene.context_scanner.current_context;

        switch (context.type)
        {
            case ContextType.FLOOR: SquadMoveCommand(context); break;
            case ContextType.COVER: SquadCoverCommand(context); break;
            case ContextType.ATTACK: SquadAttackCommand(context); break;
            case ContextType.HACK: SquadHackCommand(context); break;
        }
    }


    public void IssueFollowCommand()
    {
        squad_sense.squad_target = null;
        Transform follow_target = GameManager.scene.perspective_manager.perspective == PerspectiveMode.FPS ?
            GameManager.scene.player.transform : GameManager.scene.context_scanner.indicator_transform;

        order_target_bobber.SetTarget(follow_target);

        squad_sense.follow_targets.Clear();
        squad_sense.follow_targets.Add(follow_target);

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

    
    public void ClearAllCommands()
    {
        order_target_bobber.Deactivate();
        squad_sense.squad_target = null;

        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            squaddie.ResetOrderKnowledge();
        }
    }


    public void Update()
    {
        squad_sense.squaddies.RemoveAll(elem => elem == null);

        if (order_target_bobber.active &&
            squad_sense.squaddies.TrueForAll(elem => elem.knowledge.current_order == OrderType.NONE))
        {
            order_target_bobber.Deactivate();
        }

        EvaluateSquadCenter();
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
        order_target_bobber.SetTarget(_context.indicator_position);

        List<float> squaddie_sizes = new List<float>();
        float line_width = 0;

        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            squaddie.nav.stoppingDistance = squaddie.settings.move_stop_distance;

            line_width += squaddie.nav.radius + SQUADDIE_SPACING;
            squaddie_sizes.Add(squaddie.nav.radius);
        }

        for (int i = 0; i < num_squaddies; ++i)
        {
            float padded_squaddie = squad_sense.squaddies[i].nav.radius + SQUADDIE_SPACING;

            Vector3 waypoint = _context.indicator_position + (_context.indicator_right * (padded_squaddie * i));
            waypoint -= _context.indicator_right * ((line_width - padded_squaddie) / 2);

            squad_sense.squaddies[i].IssueMoveCommand(waypoint);
        }
    }


    void SquadCoverCommand(CurrentContext _context)
    {
        order_target_bobber.SetTarget(_context.indicator_position);

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
        order_target_bobber.SetTarget(_context.indicator_hit);

        SquaddieAI target = _context.indicator_hit.GetComponent<SquaddieAI>();
        
        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            if (JHelper.SameFaction(squaddie, target))
                continue;

            squaddie.knowledge.order_target = target;
            squaddie.knowledge.current_order = OrderType.ATTACK;
        }
    }


    void SquadHackCommand(CurrentContext _context)
    {
        order_target_bobber.SetTarget(_context.indicator_hit);

    }

}
