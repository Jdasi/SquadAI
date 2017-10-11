using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TempSceneRefs
{
    public CoverPointManager cover_point_manager
    {
        get
        {
            if (cover_point_manager_ == null)
                cover_point_manager_ = GameObject.FindObjectOfType<CoverPointManager>();

            return cover_point_manager_;
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


    private CoverPointManager cover_point_manager_;
    private ContextScanner context_scanner_;

}
