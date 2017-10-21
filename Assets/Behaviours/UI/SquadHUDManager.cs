using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadHUDManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] GameObject squad_block_prefab;

    [Header("References")]
    [SerializeField] Transform squad_grid_layout;

    private List<SquadBlock> squad_blocks = new List<SquadBlock>();


    public SquadBlock CreateUIBlock(SquadManager _squad)
    {
        GameObject clone = Instantiate(squad_block_prefab, squad_grid_layout);
            
        SquadBlock block = clone.GetComponent<SquadBlock>();
        block.Init("Squad " + (squad_blocks.Count + 1).ToString(), ref _squad);

        squad_blocks.Add(block);
        return block;
    }


    void Update()
    {
        squad_blocks.RemoveAll(elem => elem == null);
    }

}
