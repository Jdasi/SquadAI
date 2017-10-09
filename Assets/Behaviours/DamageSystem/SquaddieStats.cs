using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SquaddieStats : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] int starting_health;
    public FactionSettings faction_settings;
    [SerializeField] UnityEvent damage_events;
    [SerializeField] UnityEvent death_events;

    [Header("References")]
    [SerializeField] ShakeModule shake_module;
    [SerializeField] GameObject explosion_particle_prefab;

    [Header("Debug")]
    [SerializeField] int current_health;


    public bool alive { get { return current_health > 0; } }


    public void TakeDamage(int _amount)
    {
        if (_amount <= 0 || !alive)
            return;

        current_health -= _amount;
        damage_events.Invoke();

        if (!alive)
        {
            death_events.Invoke();
        }
    }


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


    void Start()
    {
        current_health = starting_health;
    }

}
