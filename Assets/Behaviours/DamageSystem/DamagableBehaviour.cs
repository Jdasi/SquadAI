using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DamageableBehaviour : MonoBehaviour
{
    public bool alive { get { return health > 0; } }

    [SerializeField] int starting_health;
    [SerializeField] CustomEvents.IntEvent damage_events;
    [SerializeField] UnityEvent death_events;

    protected int health;

    
    public void Damage(int _amount)
    {
        if (_amount <= 0 || !alive)
            return;

        health -= _amount;
        damage_events.Invoke(health);

        if (alive)
        {
            DerivedDamage(_amount);
        }
        else
        {
            death_events.Invoke();
            DerivedDeath();
        }
    }


    void Start()
    {
        health = starting_health;
    }


    protected abstract void DerivedDamage(int _amount);
    protected abstract void DerivedDeath();

}
