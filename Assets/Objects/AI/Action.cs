using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actions describe a particular ability.
/// The Action's preconditions must be met before Act is called by State.
/// </summary>
public abstract class Action : ScriptableObject 
{
    public abstract bool PreconditionsMet(SquaddieAI _squaddie);
    public abstract void Act(SquaddieAI _squaddie);

}
