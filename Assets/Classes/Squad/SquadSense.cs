using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SquadSense
{
    public List<SquaddieAI> squaddies = new List<SquaddieAI>();
    public Vector3 squad_center;
    public SquaddieAI squad_target;

}
