using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkingKnowledge
{
    public List<SquaddieAI> squad_members; // Reference.
    public List<SquaddieAI> nearby_targets = new List<SquaddieAI>();

    public ChainGun chain_gun;
    public float state_time_elapsed;

    public bool has_order;
    public Vector3 order_waypoint;

    public SquaddieAI closest_target;
    public SquaddieAI order_target;

}
