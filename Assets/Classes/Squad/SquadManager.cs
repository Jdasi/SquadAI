using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Encapsulates the details of a grouping of squaddies.
/// The SquadManager allows micro-management over the squaddies, whilst
/// maintaining important shared information used by the squaddies.
/// </summary>
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


    /// <summary>
    /// Primary method for issuing commands to squads.
    /// This function automatically pulls information from the ContextScanner
    /// to issue the most relevant command to the squad.
    /// </summary>
    public void IssueContextCommand()
    {
        ClearAllCommands();
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
        ClearAllCommands();
        
        // Tailor to the current PerspectiveMode.
        Transform follow_target = GameManager.scene.perspective_manager.FPSModeActive() ?
            GameManager.scene.player.transform : GameManager.scene.context_scanner.indicator_transform;

        // Visual feedback.
        order_target_bobber.SetTarget(follow_target);

        // Prepare follow lists for new information.
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

    
    /// <summary>
    /// Should be called before each new order to avoid conflicting information.
    /// Clears out information from the squad sense, and resets order-specific knowledge held by each squaddie.
    /// </summary>
    public void ClearAllCommands()
    {
        order_target_bobber.Deactivate();
        squad_sense.squad_target = null;
        squad_sense.hacker_squaddie = null;

        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            squaddie.ResetOrderKnowledge();
        }
    }


    public void Update()
    {
        // Maintain the list of squaddies.
        squad_sense.squaddies.RemoveAll(elem => elem == null);

        // Order visualisation should only display while an order is underway.
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


    /// <summary>
    /// Moves the squaddies to the context indicator position and arranges them in a line.
    /// </summary>
    /// <param name="_context">The current context.</param>
    void SquadMoveCommand(CurrentContext _context)
    {
        order_target_bobber.SetTarget(_context.indicator_position);

        float line_width = 0;

        // Prepare to arrange the squaddies in a line at the move location.
        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            line_width += squaddie.nav.radius + SQUADDIE_SPACING;
        }

        // Issue waypoints to each squaddie. The line is treated as if it's center aligned at the target location.
        for (int i = 0; i < num_squaddies; ++i)
        {
            float padded_squaddie = squad_sense.squaddies[i].nav.radius + SQUADDIE_SPACING;

            Vector3 waypoint = _context.indicator_position + (_context.indicator_right * (padded_squaddie * i));
            waypoint -= _context.indicator_right * ((line_width - padded_squaddie) / 2); // Center align.

            squad_sense.squaddies[i].IssueMoveCommand(waypoint);
        }
    }


    /// <summary>
    /// Moves the squaddies to cover near the context indicator position.
    /// A working list is used to prevent squaddies being issued the same cover points.
    /// </summary>
    /// <param name="_context"></param>
    void SquadCoverCommand(CurrentContext _context)
    {
        order_target_bobber.SetTarget(_context.indicator_position);

        // Working list to prevent squaddies being issued the same cover.
        List<CoverPoint> allocated_points = new List<CoverPoint>();

        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
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


    /// <summary>
    /// Causes the squad to engage the context indicator target.
    /// </summary>
    /// <param name="_context">The current context.</param>
    void SquadAttackCommand(CurrentContext _context)
    {
        SquaddieAI attack_target = _context.indicator_hit.GetComponent<SquaddieAI>();
        order_target_bobber.SetTarget(attack_target.transform);
        
        foreach (SquaddieAI squaddie in squad_sense.squaddies)
        {
            if (JHelper.SameFaction(squaddie, attack_target))
                continue;

            squaddie.knowledge.order_target = attack_target;
            squaddie.knowledge.current_order = OrderType.ATTACK;
        }
    }


    /// <summary>
    /// Causes the squad to move to hack the context indicator target.
    /// One squad member is randomly chosen to hack while the others occupy cover points
    /// near the target.
    /// </summary>
    /// <param name="_context"></param>
    void SquadHackCommand(CurrentContext _context)
    {
        HackableConsole console = _context.indicator_hit.GetComponent<HackableConsole>();

        if (console.hacked)
            return;

        order_target_bobber.SetTarget(console.transform);

        squad_sense.hacker_squaddie = squad_sense.squaddies[Random.Range(0, num_squaddies)];
        for (int i = 0; i < num_squaddies; ++i)
        {
            SquaddieAI squaddie = squad_sense.squaddies[i];
            squaddie.knowledge.order_console = console;

            if (squaddie == squad_sense.hacker_squaddie)
            {
                // Hacker squaddie moves to hack.
                squaddie.knowledge.current_order = OrderType.HACK;
                squaddie.nav.destination = console.hack_point.position;
            }
            else
            {
                // The other squaddies move to defend.
                Vector3 pos = JHelper.PosToCirclePos(console.hack_point.position, num_squaddies, i, 5);

                squaddie.knowledge.current_order = OrderType.GUARD;
                squaddie.MoveToCoverNearPosition(pos);
            }
        }
    }

}
