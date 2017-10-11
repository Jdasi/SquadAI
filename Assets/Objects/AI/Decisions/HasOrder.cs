using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Has Order")]
public class HasOrder : Decision
{
    public override bool Decide(SquaddieAI _squaddie)
    {
        return _squaddie.knowledge.has_order;
    }

}
