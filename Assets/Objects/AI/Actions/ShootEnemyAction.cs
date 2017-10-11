using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Shoot Enemy")]
public class ShootEnemyAction : Action
{
    public override void Act(SquaddieAI _squaddie)
    {
        ShootEnemy(_squaddie);
    }


    void ShootEnemy(SquaddieAI _squaddie)
    {
        SquaddieAI closest_target = null;
        float closest_distance = Mathf.Infinity;

        foreach (SquaddieAI target in _squaddie.knowledge.nearby_targets)
        {
            float distance = (target.transform.position - _squaddie.transform.position).sqrMagnitude;
            if (distance >= closest_distance)
                continue;

            closest_target = target;
            closest_distance = distance;
        }

        bool target_hit = false;

        if (closest_target != null)
        {
            _squaddie.transform.LookAt(closest_target.transform);

            RaycastHit hit;
            bool hit_success = Physics.Raycast(_squaddie.view_point.position, _squaddie.transform.forward, out hit, Mathf.Infinity,
                1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Damageable"));

            if (hit_success)
            {
                target_hit = hit.transform == closest_target;
            }
        }

        _squaddie.knowledge.chain_gun.cycle = target_hit;
    }

}
