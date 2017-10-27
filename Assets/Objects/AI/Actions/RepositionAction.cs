using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Reposition")]
public class RepositionAction : Action
{

    public override bool PreconditionsMet(SquaddieAI _squaddie)
    {
        OrderType current_order = _squaddie.knowledge.current_order;
        bool eligible_to_reposition = current_order == OrderType.NONE ||
            current_order == OrderType.ATTACK || current_order == OrderType.GUARD;

        if (!eligible_to_reposition || _squaddie.knowledge.current_target == null)
            return false;

        bool moving = _squaddie.nav.hasPath;
        if (!moving && (!_squaddie.knowledge.in_cover ||
            _squaddie.knowledge.position_compromised))
        {
            return true;
        }

        return false;
    }


    public override void Act(SquaddieAI _squaddie)
    {
        Reposition(_squaddie);
    }


    void Reposition(SquaddieAI _squaddie)
    {
        _squaddie.nav.isStopped = false;
        _squaddie.MoveToFlankEnemy(_squaddie.knowledge.current_target);
    }

}
