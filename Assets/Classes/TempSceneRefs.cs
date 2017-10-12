using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TempSceneRefs
{
    public TacticalAssessor tactical_assessor
    {
        get
        {
            if (tactical_assessor_ == null)
                tactical_assessor_ = GameObject.FindObjectOfType<TacticalAssessor>();

            return tactical_assessor_;
        }
    }


    public SquadSpawner squad_spawner
    {
        get
        {
            if (squad_spawner_ == null)
                squad_spawner_ = GameObject.FindObjectOfType<SquadSpawner>();

            return squad_spawner_;
        }
    }


    public CoverPointGenerator cover_point_generator
    {
        get
        {
            if (cover_point_generator_ == null)
                cover_point_generator_ = GameObject.FindObjectOfType<CoverPointGenerator>();

            return cover_point_generator_;
        }
    }


    public ContextScanner context_scanner
    {
        get
        {
            if (context_scanner_ == null)
                context_scanner_ = GameObject.FindObjectOfType<ContextScanner>();

            return context_scanner_;
        }
    }


    private TacticalAssessor tactical_assessor_;
    private SquadSpawner squad_spawner_;
    private CoverPointGenerator cover_point_generator_;
    private ContextScanner context_scanner_;

}
