using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Engage Enemy")]
public class EngageEnemyAction : Action
{
    [SerializeField] LayerMask blocking_layers;


    public override void Act(SquaddieAI _squaddie)
    {
        if (_squaddie.knowledge.closest_target == null)
            return;

        //MoveToEngage(_squaddie);
    }


    void MoveToEngage(SquaddieAI _squaddie)
    {
        SquaddieAI current_target = _squaddie.knowledge.closest_target;

        RaycastHit hit;
        Physics.Raycast(_squaddie.view_point.position, current_target.transform.position,
            out hit, Mathf.Infinity, blocking_layers);

        if (hit.collider == null)
            return;

        if (hit.collider.transform != current_target.collider_transform ||
            hit.distance > _squaddie.settings.engage_distance)
        {
            _squaddie.MoveToCoverNearPosition(current_target.transform.position);
        }
    }

}
