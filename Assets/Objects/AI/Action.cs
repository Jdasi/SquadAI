﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject 
{
    public abstract bool PreconditionsMet(SquaddieAI _squaddie);
    public abstract void Act (SquaddieAI _squaddie);

}