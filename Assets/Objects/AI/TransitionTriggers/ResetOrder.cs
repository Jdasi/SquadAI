using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Transition Triggers/Reset Order")]
public class ResetOrder : TransitionTrigger
{
    public override void Trigger(SquaddieAI _squaddie)
    {
        _squaddie.knowledge.has_order = false;
        _squaddie.knowledge.order_target = null;
        _squaddie.knowledge.order_waypoint = Vector3.zero;
    }

}
