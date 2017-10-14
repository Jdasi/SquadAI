using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Engage Enemy")]
public class EngageEnemyAction : Action
{
    [SerializeField] LayerMask blocking_layers;


    public override bool PreconditionsMet(SquaddieAI _squaddie)
    {
        if (_squaddie.knowledge.closest_target == null ||
            _squaddie.knowledge.closest_target_visible)
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
        SquaddieAI current_target = _squaddie.knowledge.closest_target;
        _squaddie.MoveToCoverNearPosition(current_target.transform.position);
    }

}
