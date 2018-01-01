using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the current context, based on what the player is looking at or hovering the mouse over.
/// Other classes can use this information to issue context sensitive commands.
/// </summary>
[System.Serializable]
public class CurrentContext
{
    public ContextType type;
    public Vector3 indicator_position;
    public Transform indicator_hit;
    public Vector3 indicator_normal;

    public Vector3 indicator_up;
    public Vector3 indicator_right;
    public Vector3 indicator_forward;

    public FactionSettings current_faction; // Unused.

}
