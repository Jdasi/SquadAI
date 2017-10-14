using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Shoot Enemy")]
public class ShootEnemyAction : Action
{
    [SerializeField] LayerMask hit_layers;


    public override void Act(SquaddieAI _squaddie)
    {
        ShootEnemy(_squaddie);
    }


    void ShootEnemy(SquaddieAI _squaddie)
    {
        Transform closest_target = null;
        float closest_distance = Mathf.Infinity;

        foreach (SquaddieAI target in _squaddie.knowledge.nearby_targets)
        {
            float distance = (target.transform.position - _squaddie.transform.position).sqrMagnitude;
            if (distance >= closest_distance)
                continue;

            closest_target = target.transform;
            closest_distance = distance;
        }

        bool target_hit = false;

        if (closest_target != null)
        {
            Vector3 dir = (closest_target.position - _squaddie.transform.position).normalized;

            RaycastHit hit;
            bool hit_success = Physics.Raycast(_squaddie.view_point.position, dir,
                out hit, _squaddie.settings.engage_distance, hit_layers);

            if (hit_success)
            {
                target_hit = hit.transform == closest_target;
            }
        }

        if (target_hit && Vector3.Distance(_squaddie.transform.position, closest_target.position)
            <= _squaddie.settings.engage_distance)
        {
            _squaddie.transform.LookAt(closest_target.transform);
            _squaddie.knowledge.chain_gun.cycle = true;
        }
        else
        {
            _squaddie.knowledge.chain_gun.cycle = false;
        }
    }

}
