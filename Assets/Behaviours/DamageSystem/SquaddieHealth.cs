using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquaddieHealth : DamageableBehaviour
{
    [Header("References")]
    [SerializeField] ShakeModule shake_module;
    [SerializeField] GameObject explosion_particle_prefab;


    public void DamageShake()
    {
        if (shake_module == null)
            return;

        shake_module.Shake(0.1f, 0.1f);
    }


    public void DestroySquaddie()
    {
        if (explosion_particle_prefab != null)
            Instantiate(explosion_particle_prefab, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
    }


    protected override void DerivedDamage(int _amount)
    {
        
    }

    protected override void DerivedDeath()
    {
        
    }

}
