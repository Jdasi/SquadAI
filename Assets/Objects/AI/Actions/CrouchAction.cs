using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Crouch")]
public class CrouchAction : Action
{

    public override bool PreconditionsMet(SquaddieAI _squaddie)
    {
        return true;
    }


    public override void Act(SquaddieAI _squaddie)
    {
        _squaddie.knowledge.crouched = _squaddie.knowledge.in_cover &&
            _squaddie.knowledge.current_target_visible;

        _squaddie.knowledge.crouched &= !_squaddie.nav.hasPath;
    }

}
