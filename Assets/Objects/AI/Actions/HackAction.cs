using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Hack")]
public class HackAction : Action
{

    public override bool PreconditionsMet(SquaddieAI _squaddie)
    {
        bool ordered_to_hack = _squaddie.knowledge.current_order == OrderType.HACK;
        bool nearby_console_exists = _squaddie.knowledge.nearby_console != null;

        if (!ordered_to_hack || !nearby_console_exists)
        {
            _squaddie.knowledge.hacking = false;
            return false;
        }

        return true;
    }


    public override void Act(SquaddieAI _squaddie)
    {
        _squaddie.knowledge.hacking = true;
        _squaddie.knowledge.nearby_console.Hack(_squaddie);
    }

}
