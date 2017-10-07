using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSpawner : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] KeyCode spawn_squaddie_key = KeyCode.RightBracket;
    [SerializeField] KeyCode remove_squaddie_key = KeyCode.LeftBracket;
    [SerializeField] GameObject squaddie_prefab;
    [SerializeField] FactionSettings spawn_faction;

    [Header("References")]
    [SerializeField] SquadControl player_squad_control;


    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(spawn_squaddie_key))
            SpawnSquaddie();

        if (Input.GetKeyDown(remove_squaddie_key))
        {
            if (player_squad_control.issuing_order)
                player_squad_control.RemoveSquaddie();
        }
    }


    void SpawnSquaddie()
    {
        ContextType current_context_type = GameManager.scene.context_scanner.current_context.type;
        Vector3 indicator_position = GameManager.scene.context_scanner.current_context.indicator_position;

        if (!player_squad_control.issuing_order || current_context_type != ContextType.FLOOR)
            return;

        GameObject clone = Instantiate(squaddie_prefab, indicator_position, Quaternion.identity);

        clone.GetComponent<SquaddieStats>().faction_settings = spawn_faction;

        var agent = clone.GetComponent<SquaddieAgent>();
        agent.Init();

        player_squad_control.AddSquaddie(agent);
    }


}
