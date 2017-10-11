using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Order Obeyed")]
public class OrderObeyedDecision : Decision
{
    public override bool Decide(SquaddieAI _squaddie)
    {
        bool near_waypoint = Vector3.Distance(_squaddie.transform.position,
            _squaddie.knowledge.order_waypoint) <= _squaddie.nav.stoppingDistance;

        bool no_target = _squaddie.knowledge.order_target == null;

        bool order_obeyed = (near_waypoint && no_target) || !_squaddie.nav.hasPath;
        return order_obeyed;
    }

}
