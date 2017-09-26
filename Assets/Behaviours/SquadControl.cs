using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadControl : MonoBehaviour
{
    [Header("Parameters")]
    [Range(0, 10)][SerializeField] int max_squads;

    [Header("References")]
    [SerializeField] ContextScanner context_scanner;
    [SerializeField] SquadHUDManager squad_hud_manager;

    private List<Squad> squads = new List<Squad>();
    private int selected_squad_index;


    public void AddSquaddie(Squaddie _squaddie, int _squad_number = -1)
    {
        if (_squad_number >= squads.Count)
            return;
        else if (_squad_number < 0)
            _squad_number = selected_squad_index;

        squads[_squad_number].AddSquaddie(_squaddie);
        squad_hud_manager.UpdateSquadBlockUnitCount(_squad_number,
            squads[_squad_number].num_squaddies);
    }


    public void RemoveSquaddie(int _squad_number = -1)
    {
        if (_squad_number < 0)
            _squad_number = selected_squad_index;

        squads[_squad_number].RemoveSquaddie();
        squad_hud_manager.UpdateSquadBlockUnitCount(_squad_number,
            squads[_squad_number].num_squaddies);
    }


    void Start()
    {
        max_squads = Mathf.Clamp(max_squads, 0, 10);

        for (int i = 0; i < max_squads; ++i)
            squads.Add(new Squad());

        squad_hud_manager.InitSquadBlocks(squads.Count);
        squad_hud_manager.SelectSquadBlock(0);
    }


    void Update()
    {
        HandleSquadSelection();

        if (Input.GetButtonDown("Command"))
            IssueContextCommand();

        foreach (Squad squad in squads)
            squad.Update();
    }


    void HandleSquadSelection()
    {
        EvaluateSquadSelectionChange(KeyCode.Alpha0);
        EvaluateSquadSelectionChange(KeyCode.Alpha1);
        EvaluateSquadSelectionChange(KeyCode.Alpha2);
        EvaluateSquadSelectionChange(KeyCode.Alpha3);
        EvaluateSquadSelectionChange(KeyCode.Alpha4);
        EvaluateSquadSelectionChange(KeyCode.Alpha5);
        EvaluateSquadSelectionChange(KeyCode.Alpha6);
        EvaluateSquadSelectionChange(KeyCode.Alpha7);
        EvaluateSquadSelectionChange(KeyCode.Alpha8);
        EvaluateSquadSelectionChange(KeyCode.Alpha9);
    }


    void EvaluateSquadSelectionChange(KeyCode _code)
    {
        if (!Input.GetKeyDown(_code))
            return;

        /* Match squad selection to the arrangement of number keys on the keyboard.
         * (i.e. shift all numbers one place to the left and make 0 the last selection)
         */
        int squad_number = (int)_code - (int)KeyCode.Alpha1;

        if (squad_number == -1)
            squad_number = 9;

        ChangeSquadSelection(squad_number);
        squad_hud_manager.SelectSquadBlock(squad_number);
    }


    void ChangeSquadSelection(int _squad_number)
    {
        if (_squad_number >= squads.Count || _squad_number < 0)
            return;

        selected_squad_index = _squad_number;
    }


    void IssueContextCommand()
    {
        if (selected_squad_index >= squads.Count)
            return;

        ContextCommand command = new ContextCommand();

        command.type = context_scanner.current_context;
        command.target = context_scanner.indicator_position;

        command.direction = command.target - transform.position;
        command.direction = (command.direction - new Vector3(0, command.direction.y)).normalized;

        squads[selected_squad_index].IssueContextCommand(command);
    }

}
