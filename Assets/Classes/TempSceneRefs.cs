using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TempSceneRefs
{
    public SceneArranger scene_arranger
    {
        get
        {
            if (scene_arranger_ == null)
                scene_arranger_ = GameObject.FindObjectOfType<SceneArranger>();

            return scene_arranger_;
        }
    }


    public PerspectiveManager perspective_manager
    {
        get
        {
            if (perspective_manager_ == null)
                perspective_manager_ = GameObject.FindObjectOfType<PerspectiveManager>();

            return perspective_manager_;
        }
    }


    public PlayerSquadControl player_squad_control
    {
        get
        {
            if (player_squad_control_ == null)
                player_squad_control_ = GameObject.FindObjectOfType<PlayerSquadControl>();

            return player_squad_control_;
        }
    }


    public FirstPersonMovement player
    {
        get
        {
            if (player_ == null)
                player_ = GameObject.FindObjectOfType<FirstPersonMovement>();

            return player_;
        }
    }


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


    private PerspectiveManager perspective_manager_;
    private PlayerSquadControl player_squad_control_;
    private FirstPersonMovement player_;
    private TacticalAssessor tactical_assessor_;
    private SquadSpawner squad_spawner_;
    private CoverPointGenerator cover_point_generator_;
    private ContextScanner context_scanner_;
    private SceneArranger scene_arranger_;

}
