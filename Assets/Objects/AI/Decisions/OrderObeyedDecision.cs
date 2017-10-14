using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Order Obeyed")]
public class OrderObeyedDecision : Decision
{
    public override bool Decide(SquaddieAI _squaddie)
    {
        bool order_obeyed = true;

        if (_squaddie.knowledge.current_order == OrderType.MOVE)
        {
            order_obeyed &= Vector3.Distance(_squaddie.transform.position,
                _squaddie.knowledge.order_waypoint) <= _squaddie.nav.stoppingDistance +
                _squaddie.nav.radius;
        }
        else if (_squaddie.knowledge.current_order == OrderType.ATTACK)
        {
            order_obeyed &= _squaddie.knowledge.order_target == null;
        }
        else if (_squaddie.knowledge.current_order == OrderType.FOLLOW)
        {
            order_obeyed = false;
        }

        return order_obeyed;
    }

}
