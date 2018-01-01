using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum OrderType
{
    NONE,
    MOVE,
    FOLLOW,
    ATTACK,
    HACK,
    GUARD
}

public class PlayerSquadControl : MonoBehaviour
{
    public bool issuing_order { get; private set; }
    public int squad_count { get { return squads.Count; } }

    [Header("References")]
    [SerializeField] SquadHUDManager squad_hud_manager;
    [SerializeField] GameObject target_bobber_prefab;

    [Header("Debug")]
    [SerializeField] int squad_spawn_size;
    [SerializeField] SpawnSettings[] spawn_settings;

    [Space]
    [SerializeField] KeyCode remove_squaddie_key = KeyCode.Backspace;

    private List<SquadManager> squads = new List<SquadManager>();
    private SquadManager selected_squad;


    public void AddSquad(SquadManager _squad)
    {
        _squad.AssignTargetBobber(Instantiate(target_bobber_prefab));
        squad_hud_manager.CreateUIBlock(_squad);

        squads.Add(_squad);
    }


    void Update()
    {
        if (issuing_order && selected_squad.num_squaddies == 0)
            OrderFinished();

        squads.RemoveAll(elem => elem.num_squaddies == 0);

        HandleSquadSpawning();
        HandleSquadRemoval();
        HandleSquadSelection();

        if (Input.GetButtonDown("Command"))
            IssueContextCommand();

        if (Input.GetButtonDown("CancelOrder"))
            OrderFinished();
        
        if (Input.GetKeyDown(KeyCode.F))
            IssueFollowCommand();

        UpdateAllSquads();
    }


    void HandleSquadSpawning()
    {
        SquadSpawner squad_spawner = GameManager.scene.squad_spawner;

        foreach (SpawnSettings settings in spawn_settings)
        {
            if (!Input.GetKeyDown(settings.spawn_key))
                continue;

            if (selected_squad == null)
            {
                SquadSpawn(settings, squad_spawner);
            }
            else
            {
                IndividualSpawn(settings, squad_spawner);
            }
        }
    }


    void SquadSpawn(SpawnSettings _settings, SquadSpawner _spawner)
    {
        RaycastHit hit;
        if (!JHelper.RaycastCameraToFloor(out hit))
            return;

        AddSquad(_spawner.CreateSquad(_settings.faction, squad_spawn_size, hit.point));
    }


    void IndividualSpawn(SpawnSettings _settings, SquadSpawner _spawner)
    {
        RaycastHit hit;
        if (!JHelper.RaycastCameraToFloor(out hit))
            return;

        selected_squad.AddSquaddie(_spawner.CreateSquaddie(_settings.faction, hit.point));
        selected_squad.SelectSquad();
    }


    void HandleSquadRemoval()
    {
        if (selected_squad == null || !Input.GetKeyDown(remove_squaddie_key))
            return;

        selected_squad.RemoveSquaddie();
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

        SquadManager target_squad = squads[_squad_number];

        if (selected_squad != target_squad)
            OrderFinished();

        selected_squad = target_squad;
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
    }


    void OrderFinished()
    {
        if (selected_squad == null)
            return;

        bool squad_following = selected_squad.squad_sense.squaddies.Any(
            elem => elem.knowledge.current_order == OrderType.FOLLOW);

        if (squad_following)
            selected_squad.ClearAllCommands();

        selected_squad.DeselectSquad();
        selected_squad = null;

        issuing_order = false;
        GameManager.scene.context_scanner.Deactivate();
    }

}
