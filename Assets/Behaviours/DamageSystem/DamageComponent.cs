using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
