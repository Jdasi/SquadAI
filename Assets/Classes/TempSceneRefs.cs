using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TempSceneRefs
{
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


    private CoverPointGenerator cover_point_generator_;
    private ContextScanner context_scanner_;

}
