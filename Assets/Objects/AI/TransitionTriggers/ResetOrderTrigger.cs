using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Transition Triggers/Reset Order")]
public class ResetOrderTrigger : TransitionTrigger
{
    public override void Trigger(SquaddieAI _squaddie)
    {
        _squaddie.knowledge.current_order = OrderType.NONE;
        _squaddie.knowledge.order_waypoint = Vector3.zero;
        _squaddie.knowledge.hacking = false;
        _squaddie.knowledge.squad_sense.hacker_squaddie = null;
    }

}
