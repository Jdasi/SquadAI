using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Custom/Line of Sight Weight")]
public class LineOfSightWeight : WeightingModule
{
    [SerializeField] LayerMask blocking_layers;


    public override void AdjustWeight(CoverPoint _cover_point, SquaddieAI _caller)
    {
        List<SquaddieAI> squaddies = GameManager.scene.squad_spawner.all_units;

        foreach (SquaddieAI squaddie in squaddies)
        {
            if (JHelper.SameFaction(squaddie, _caller))
                continue;

            float distance = Vector3.Distance(squaddie.view_point.position, _cover_point.position);
            Vector3 direction = _cover_point.position - squaddie.view_point.position;

            if (Physics.Raycast(squaddie.view_point.position, direction, distance, blocking_layers))
                continue;

            _cover_point.weighting += base.true_adjustment;
        }
    }

}
