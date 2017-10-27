using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Order Obeyed")]
public class OrderObeyedDecision : Decision
{
    public override bool Decide(SquaddieAI _squaddie)
    {
        bool order_obeyed = true;

        switch (_squaddie.knowledge.current_order)
        {
            case OrderType.MOVE:
            {
                order_obeyed &= !_squaddie.nav.hasPath;
            } break;

            case OrderType.FOLLOW:
            {
                order_obeyed = false;
            } break;

            case OrderType.ATTACK:
            {
                order_obeyed &= _squaddie.knowledge.order_target == null;
            } break;

            case OrderType.HACK:
            {
                order_obeyed &= _squaddie.knowledge.order_console.hacked;
            } break;

            case OrderType.GUARD:
            {
                order_obeyed &= _squaddie.knowledge.squad_sense.hacker_squaddie == null;
            } break;
        }

        return order_obeyed;
    }

}
