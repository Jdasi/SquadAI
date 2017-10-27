using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkingKnowledge
{
    public SquadSense squad_sense; // Reference.
    public List<SquaddieAI> nearby_targets = new List<SquaddieAI>();
    public RaycastHit sight_hit;

    public ChainGun chain_gun;
    public float state_time_elapsed;
    public float prev_state_time_elapsed;

    public OrderType current_order;
    public Vector3 order_waypoint;
    public Transform follow_target;

    public SquaddieAI closest_target;
    public SquaddieAI order_target;
    public SquaddieAI current_target;

    public bool current_target_visible;
    public bool current_target_in_range;
    public bool crouched;
    public bool in_cover;
    public bool position_compromised;

    public HackableConsole order_console;
    public HackableConsole nearby_console;
    public bool hacking;

}
