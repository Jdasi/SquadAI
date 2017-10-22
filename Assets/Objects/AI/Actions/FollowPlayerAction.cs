using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Follow Player")]
public class FollowPlayerAction : Action
{

    public override bool PreconditionsMet(SquaddieAI _squaddie)
    {
        bool following = _squaddie.knowledge.current_order == OrderType.FOLLOW;
        return following;
    }


    public override void Act(SquaddieAI _squaddie)
    {
        if (_squaddie.knowledge.follow_target == null)
        {
            if (!FindNewFollowTarget(_squaddie))
                return;
        }

        _squaddie.nav.isStopped = _squaddie.nav.remainingDistance <= _squaddie.nav.stoppingDistance;
        _squaddie.nav.destination = _squaddie.knowledge.follow_target.position;
    }


    bool FindNewFollowTarget(SquaddieAI _squaddie)
    {
        if (_squaddie.knowledge.follow_target == null)
        {
            foreach (SquaddieAI ally in _squaddie.knowledge.squad_sense.squaddies)
            {
                if (ally == _squaddie ||
                    ally.knowledge.follow_target == _squaddie.transform)
                {
                    continue;
                }

                _squaddie.knowledge.follow_target = ally.transform;
                break;
            }
        }

        if (_squaddie.knowledge.follow_target == null)
        {
            Transform follow_target = GameManager.scene.perspective_manager.perspective == PerspectiveMode.FPS ?
                GameManager.scene.player.transform : GameManager.scene.context_scanner.indicator_transform;

            _squaddie.knowledge.follow_target = follow_target;
        }

        return _squaddie.knowledge.follow_target != null;
    }

}
