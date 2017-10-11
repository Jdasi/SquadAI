using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeightingModule : ScriptableObject
{
    [SerializeField] protected float true_adjustment;
    [SerializeField] protected float false_adjustment;

    public abstract void AdjustWeight(CoverPoint _cover_point, SquaddieAI _caller);

}
