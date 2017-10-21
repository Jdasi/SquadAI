using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum OrderType
{
    NONE,
    MOVE,
    FOLLOW,
    ATTACK
}

public class PlayerSquadControl : MonoBehaviour
{
    public bool issuing_order { get; private set; }

    [Header("References")]
    [SerializeField] SquadHUDManager squad_hud_manager;

    [Header("Debug")]
    [SerializeField] SpawnSettings[] spawn_settings;

    [Space]
    [SerializeField] KeyCode remove_squaddie_key = KeyCode.Backspace;

    private List<SquadManager> squads = new List<SquadManager>();
    private SquadManager selected_squad;


    void Update()
    {
        squads.RemoveAll(elem => elem.num_squaddies == 0);

        HandleSquadSpawning();
        HandleSquadRemoval();
        HandleSquadSelection();

        if (Input.GetButtonDown("Command"))
            IssueContextCommand();

        if (Input.GetButtonDown("CancelOrder"))
            OrderFinished();
        
        /* COMMENTED OUT UNTIL NEW FOLLOW COMMAND IS IMPLEMENTED.
        if (Input.GetKeyDown(KeyCode.F))
            IssueFollowCommand();
        */

        UpdateAllSquads();
    }


    void HandleSquadSpawning()
    {
        SquadSpawner squad_spawner = GameManager.scene.squad_spawner;

        foreach (SpawnSettings settings in spawn_settings)
        {
            if (Input.GetKeyDown(settings.spawn_key))
            {
                Ray ray = JHelper.main_camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool _ray_success = Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Floor"));

                if (!_ray_success)
                    break;

                Vector3 mouse_position = hit.point;

                SquadManager squad = squad_spawner.CreateSquad(settings.faction, 4, mouse_position);
                squad_hud_manager.CreateUIBlock(squad);

                squads.Add(squad);
            }
        }
    }


    void HandleSquadRemoval()
    {
        if (selected_squad == null || !Input.GetKeyDown(remove_squaddie_key))
            return;

        foreach (SquaddieAI squaddie in selected_squad.squad_sense.squaddies)
            Destroy(squaddie.gameObject);

        OrderFinished();
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


    void UpdateAllSquads()
    {
        foreach (SquadManager squad in squads)
            squad.Update();
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

        if (squad_number >= squads.Count || squad_number < 0)
            return;

        ChangeSquadSelection(squad_number);

        issuing_order = true;
        GameManager.scene.context_scanner.Activate(squads[squad_number].squad_faction);
    }


    void ChangeSquadSelection(int _squad_number)
    {
        if (selected_squad != null)
            selected_squad.DeselectSquad();

        selected_squad = squads[_squad_number];
        selected_squad.SelectSquad();
    }


    void IssueContextCommand()
    {
        if (selected_squad == null)
            return;

        selected_squad.IssueContextCommand();

        OrderFinished();
    }


    void IssueFollowCommand()
    {
        if (selected_squad == null)
            return;

        selected_squad.IssueFollowCommand();

        OrderFinished();
    }


    void OrderFinished()
    {
        if (selected_squad == null)
            return;

        selected_squad.DeselectSquad();
        issuing_order = false;

        GameManager.scene.context_scanner.Deactivate();
    }

}
