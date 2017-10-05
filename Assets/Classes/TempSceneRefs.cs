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


    private CoverPointGenerator cover_point_generator_;

}
