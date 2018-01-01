using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for specialised Decisions to derive from.
/// All Decisions return a boolean which is used by a State to determine
/// a transition to a new state.
/// </summary>
public abstract class Decision : ScriptableObject
{
    public abstract bool Decide(SquaddieAI _squaddie);

}
