using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Shoot Enemy")]
public class ShootEnemyAction : Action
{

    public override bool PreconditionsMet(SquaddieAI _squaddie)
    {
        bool target_exists = _squaddie.knowledge.current_target != null;

        if (!target_exists || !_squaddie.knowledge.current_target_visible)
        {
            _squaddie.knowledge.chain_gun.cycle = false;
            return false;
        }

        return true;
    }


    public override void Act(SquaddieAI _squaddie)
    {
        ShootEnemy(_squaddie);
    }


    void ShootEnemy(SquaddieAI _squaddie)
    {
        Quaternion rot = Quaternion.LookRotation(_squaddie.knowledge.current_target.transform.position -
            _squaddie.transform.position);
        _squaddie.transform.rotation = Quaternion.RotateTowards(_squaddie.transform.rotation,
            rot, (_squaddie.nav.angularSpeed * 2) * Time.deltaTime);

        _squaddie.knowledge.chain_gun.cycle = _squaddie.knowledge.sight_hit.transform ==
            _squaddie.knowledge.current_target.transform;
    }

}
