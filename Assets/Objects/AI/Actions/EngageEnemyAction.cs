using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Engage Enemy")]
public class EngageEnemyAction : Action
{

    public override bool PreconditionsMet(SquaddieAI _squaddie)
    {
        OrderType current_order = _squaddie.knowledge.current_order;
        bool eligible_to_engage = current_order == OrderType.NONE ||
            current_order == OrderType.ATTACK;

        if (!eligible_to_engage ||
            _squaddie.knowledge.current_target == null ||
            (_squaddie.knowledge.current_target_visible &&
            _squaddie.knowledge.current_target_in_range))
        {
            return false;
        }

        return true;
    }


    public override void Act(SquaddieAI _squaddie)
    {
        MoveToEngage(_squaddie);
    }


    void MoveToEngage(SquaddieAI _squaddie)
    {
        if (_squaddie.knowledge.squad_sense.squad_target == null)
            _squaddie.knowledge.squad_sense.squad_target = _squaddie.knowledge.current_target;

        _squaddie.nav.isStopped = false;
        _squaddie.MoveToFlankEnemy(_squaddie.knowledge.current_target);
    }

}
