using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Transition Triggers/Deactivate Gun")]
public class DeactivateGunTrigger : TransitionTrigger
{
    public override void Trigger(SquaddieAI _squaddie)
    {
        _squaddie.knowledge.chain_gun.cycle = false;
    }

}
