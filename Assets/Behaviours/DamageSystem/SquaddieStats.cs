using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SquaddieStats : MonoBehaviour
{
    public bool alive { get { return current_health > 0; } }

    [Header("Parameters")]
    public FactionSettings faction_settings;                // Determines the squaddie's alliegance and appearance.
    [SerializeField] int starting_health;                   // How much health the squaddie starts with.
    [SerializeField] UnityEvent damage_events;              // Events that fire whenever the squaddie takes damage.
    [SerializeField] UnityEvent death_events;               // Events that fire when the squaddie's health is reduced to 0.

    [Header("References")]
    [SerializeField] ShakeModule shake_module;
    [SerializeField] GameObject explosion_particle_prefab;

    [Header("Debug")]
    [SerializeField] int current_health;


    /// <summary>
    /// Deals damage to the squaddie.
    /// If the damage reduces the squaddie's health to 0, the squaddie dies.
    /// </summary>
    /// <param name="_amount">The amount of damage to be dealt.</param>
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


    /// <summary>
    /// Shakes the squaddie a small amount.
    /// </summary>
    public void DamageShake()
    {
        if (shake_module == null)
            return;

        shake_module.Shake(0.1f, 0.1f);
    }


    /// <summary>
    /// Creates an explosion and destroys the squaddie.
    /// </summary>
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
