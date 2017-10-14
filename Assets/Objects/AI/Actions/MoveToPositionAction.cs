using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (_squaddie.nav.isStopped && _squaddie.nav.hasPath)
            _squaddie.nav.isStopped = false;

        if (_squaddie.nav.hasPath && _squaddie.nav.remainingDistance <= _squaddie.nav.stoppingDistance)
        {
            _squaddie.nav.isStopped = true;
            _squaddie.nav.ResetPath();
        }
    }

}
