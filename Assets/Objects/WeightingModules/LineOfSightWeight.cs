using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Custom/Weighting Modules/Line of Sight")]
public class LineOfSightWeight : WeightingModule
{
    [SerializeField] LayerMask blocking_layers;
    [SerializeField] float vulnerability_adjustment;
    [SerializeField] float flanking_adjustment;


    public override void AdjustWeight(CoverPoint _cover_point, CurrentContext _context, SquaddieAI _squaddie)
    {
        foreach (SquaddieAI enemy in _squaddie.knowledge.nearby_targets)
        {
            if (JHelper.SameFaction(enemy, _squaddie))
                continue;

            float distance = Vector3.Distance(enemy.view_point.position, _cover_point.position);
            Vector3 direction = (_cover_point.position - enemy.view_point.position).normalized;

            if (EnemyHasLOS(_cover_point, _squaddie, enemy, direction, distance))
            {
                _cover_point.weighting += vulnerability_adjustment;
            }
            else if (GoodFlankSpot(_cover_point, _squaddie, enemy, distance))
            {
                _cover_point.weighting += flanking_adjustment;
            }
        }
    }

    
    bool EnemyHasLOS(CoverPoint _cover_point, SquaddieAI _squaddie, SquaddieAI _enemy,
        Vector3 _direction, float _distance)
    {
        RaycastHit enemy_ray;
        bool enemy_ray_blocked = Physics.Raycast(_enemy.view_point.position, _direction,
            out enemy_ray, _distance, blocking_layers);

        if (enemy_ray_blocked)
            return false;

        return true;
    }


    bool GoodFlankSpot(CoverPoint _cover_point, SquaddieAI _squaddie, SquaddieAI _enemy,
        float _distance)
    {
        Vector3 view_pos = _cover_point.position;
        view_pos.y = _squaddie.view_point.position.y;

        Vector3 flank_dir = (_enemy.view_point.position - view_pos).normalized;

        RaycastHit squaddie_ray;
        bool squaddie_ray_blocked = Physics.Raycast(view_pos, flank_dir, out squaddie_ray,
            _distance, blocking_layers);

        if (squaddie_ray_blocked)
            return false;

        return true;
    }
}
