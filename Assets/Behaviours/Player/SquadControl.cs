using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrderType
{
    NONE,
    MOVE,
    FOLLOW,
    ATTACK
}

public class SquadControl : MonoBehaviour
{
    public bool issuing_order { get; private set; }

    [Header("Parameters")]
    [Range(0, 10)][SerializeField] int max_squads;
    [SerializeField] SquadSettings settings;

    [Header("References")]
    [SerializeField] ContextScanner context_scanner;
    [SerializeField] SquadHUDManager squad_hud_manager;

    private List<SquadManager> squads = new List<SquadManager>();
    private int selected_squad_index;


    public void AddSquaddie(SquaddieAI _squaddie_ai, int _squad_number = -1)
    {
        if (_squad_number >= squads.Count)
            return;
        else if (_squad_number < 0)
            _squad_number = selected_squad_index;

        // Debug contingency.
        if (squads[_squad_number].num_squaddies >= settings.max_squaddies)
        {
            Destroy(_squaddie_ai.gameObject);
            return;
        }

        squads[_squad_number].AddSquaddie(_squaddie_ai);
    }


    public void RemoveSquaddie(int _squad_number = -1)
    {
        if (_squad_number < 0)
            _squad_number = selected_squad_index;

        squads[_squad_number].RemoveSquaddie();
    }


    void Start()
    {
        max_squads = Mathf.Clamp(max_squads, 0, 10);
        squad_hud_manager.InitSquadBlocks(max_squads);

        for (int i = 0; i < max_squads; ++i)
        {
            SquadBlock ui_block = squad_hud_manager.GetSquadBlock(i);
            squads.Add(new SquadManager(settings, ref ui_block));
        }
    }


    void Update()
    {
        HandleSquadSelection();

        if (Input.GetButtonDown("Command"))
            IssueContextCommand();

        if (Input.GetButtonDown("CancelOrder"))
            ResetSelection();
        else if (Input.GetKeyDown(KeyCode.F))
            IssueFollowCommand();

        foreach (SquadManager squad in squads)
            squad.Update();

        context_scanner.enabled = issuing_order;
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

        issuing_order = true;
    }


    void ChangeSquadSelection(int _squad_number)
    {
        squads[selected_squad_index].DeselectSquad();

        if (_squad_number >= squads.Count || _squad_number < 0)
            _squad_number = 0;

        selected_squad_index = _squad_number;
        squads[selected_squad_index].SelectSquad();
    }


    void IssueContextCommand()
    {
        if (selected_squad_index >= squads.Count)
            return;

        squads[selected_squad_index].IssueContextCommand(context_scanner.current_context);

        ResetSelection();
    }


    void IssueFollowCommand()
    {
        if (selected_squad_index >= squads.Count)
            return;

        squads[selected_squad_index].IssueFollowCommand();

        ResetSelection();
    }


    void ResetSelection()
    {
        if (selected_squad_index >= squads.Count)
            return;

        squads[selected_squad_index].DeselectSquad();
        issuing_order = false;
    }

}
