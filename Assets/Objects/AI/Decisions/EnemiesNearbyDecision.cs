using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Enemies Nearby")]
public class EnemiesNearbyDecision : Decision
{
    public override bool Decide(SquaddieAI _squaddie)
    {
        bool enemies_nearby = _squaddie.knowledge.nearby_targets.Count > 0;
        return enemies_nearby;
    }

}
