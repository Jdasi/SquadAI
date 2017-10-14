using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Has Order")]
public class HasOrderDecision : Decision
{
    public override bool Decide(SquaddieAI _squaddie)
    {
        return _squaddie.knowledge.current_order != OrderType.NONE;
    }

}
