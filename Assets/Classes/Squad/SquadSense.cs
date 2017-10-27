using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SquadSense
{
    public FactionSettings faction;

    public List<SquaddieAI> squaddies = new List<SquaddieAI>();
    public Vector3 squad_center;

    public List<Transform> follow_targets = new List<Transform>();
    public SquaddieAI squad_target;
    public SquaddieAI hacker_squaddie;

}
