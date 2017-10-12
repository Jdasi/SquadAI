using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeightingModule : ScriptableObject
{
    public abstract void AdjustWeight(CoverPoint _cover_point, CurrentContext _context, SquaddieAI _squaddie);

}
