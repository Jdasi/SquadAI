using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Move To Position")]
public class MoveToPositionAction : Action
{

    public override bool PreconditionsMet(SquaddieAI _squaddie)
    {
        return true;
    }


    public override void Act(SquaddieAI _squaddie)
    {
        MoveToPosition(_squaddie);
    }


    void MoveToPosition(SquaddieAI _squaddie)
    {
        UpdateHackPosition(_squaddie);

        if (_squaddie.nav.hasPath && _squaddie.nav.remainingDistance <=
            _squaddie.nav.stoppingDistance)
        {
            _squaddie.nav.isStopped = true;
            _squaddie.nav.ResetPath();
        }
        else
        {
            _squaddie.nav.isStopped = false;
        }
    }


    void UpdateHackPosition(SquaddieAI _squaddie)
    {
        if (_squaddie.knowledge.current_order == OrderType.HACK)
        {
            HackableConsole console = _squaddie.knowledge.order_console;
            float dist = Vector3.Distance(_squaddie.transform.position,
                console.transform.position);

            if (dist > _squaddie.nav.stoppingDistance)
            {
                _squaddie.nav.destination = console.hack_point.position;
                _squaddie.nav.isStopped = false;
            }
        }
    }

}
