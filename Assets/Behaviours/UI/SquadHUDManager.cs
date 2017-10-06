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


    public void InitSquadBlocks(int _amount)
    {
        for (int i = 0; i < _amount; ++i)
        {
            GameObject clone = Instantiate(squad_block_prefab, squad_grid_layout);
            
            SquadBlock block = clone.GetComponent<SquadBlock>();
            block.Init(i);

            squad_blocks.Add(block);
        }
    }


    public SquadBlock GetSquadBlock(int _index)
    {
        if (!IndexValid(_index))
            return null;

        return squad_blocks[_index];
    }


    public void SelectSquadBlock(int _block_index)
    {
        if (!IndexValid(_block_index))
            return;

        foreach (SquadBlock block in squad_blocks)
            block.Deselect();

        squad_blocks[_block_index].Select();
    }


    public void UpdateSquadBlockUnitCount(int _block_index, int _unit_count)
    {
        if (!IndexValid(_block_index))
            return;

        squad_blocks[_block_index].UpdateUnitCount(_unit_count);
    }


    bool IndexValid(int _index)
    {
        return _index >= 0 && _index < squad_blocks.Count;
    }

}
