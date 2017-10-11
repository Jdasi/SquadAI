using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TransitionTrigger : ScriptableObject
{
    public abstract void Trigger(SquaddieAI _squaddie);

}
