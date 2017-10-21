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
            order_obeyed &= !_squaddie.nav.hasPath;
        }
        else if (_squaddie.knowledge.current_order == OrderType.ATTACK)
        {
            order_obeyed &= _squaddie.knowledge.order_target == null;
        }
        else if (_squaddie.knowledge.current_order == OrderType.FOLLOW)
        {
            // Follow until new order.
            order_obeyed = false;
        }

        return order_obeyed;
    }

}
