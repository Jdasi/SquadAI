using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Order Cancelled")]
public class OrderCancelledDecision : Decision
{
    public override bool Decide(SquaddieAI _squaddie)
    {
        float distance = (_squaddie.nav.destination - _squaddie.knowledge.order_waypoint).sqrMagnitude;
        bool waypoint_different = distance > 0.1f;

        bool order_cancelled = waypoint_different;
        return order_cancelled;
    }

}
