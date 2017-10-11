using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class Transition 
{
    public Decision decision;

    [Space]
    public State true_state;
    public TransitionTrigger[] true_triggers;

    [Space]
    public State false_state;
    public TransitionTrigger[] false_triggers;

}
