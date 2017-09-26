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

    private List<Squad> squads = new List<Squad>();
    private int selected_squad_index;


    public void AddSquaddie(Squaddie _squaddie, int _squad_number = -1)
    {
        if (_squad_number >= squads.Count)
            return;
        else if (_squad_number < 0)
            _squad_number = selected_squad_index;

        squads[_squad_number].AddSquaddie(_squaddie);
    }


    void Start()
    {
        max_squads = Mathf.Clamp(max_squads, 0, 10);

        for (int i = 0; i < max_squads; ++i)
            squads.Add(new Squad());
    }


    void Update()
    {
        if (Input.GetButtonDown("Command"))
            IssueContextCommand();

        foreach (Squad squad in squads)
            squad.Update();
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
