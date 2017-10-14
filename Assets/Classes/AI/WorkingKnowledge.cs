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

    public OrderType current_order;
    public Vector3 order_waypoint;

    public SquaddieAI closest_target;
    public SquaddieAI order_target;

    public bool closest_target_visible;

}
