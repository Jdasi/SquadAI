using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TransitionTriggers work in conjunction with Decisions.
/// When a Decision would cause a State transition, any associated TransitionTriggers
/// will fire automatically. These are useful for resetting information or cleaning up data.
/// </summary>
public abstract class TransitionTrigger : ScriptableObject
{
    public abstract void Trigger(SquaddieAI _squaddie);

}
