using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This behaviour should be added to objects that can be damaged.
/// Note: ensure that the object is also on the Damageable layer.
/// </summary>
public class DamageComponent : MonoBehaviour
{
    [SerializeField] CustomEvents.IntEvent damage_events;
    

    public void Damage(int _amount)
    {
        if (_amount <= 0)
            return;

        damage_events.Invoke(_amount);
    }

}
