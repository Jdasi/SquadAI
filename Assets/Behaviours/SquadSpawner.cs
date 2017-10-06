using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSpawner : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] KeyCode spawn_squaddie_key = KeyCode.RightBracket;
    [SerializeField] KeyCode remove_squaddie_key = KeyCode.LeftBracket;
    [SerializeField] GameObject squaddie_prefab;
    [SerializeField] Vector3 squaddie_spawn_point;

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
            player_squad_control.RemoveSquaddie();
    }


    void SpawnSquaddie()
    {
        if (!player_squad_control.issuing_order)
            return;

        GameObject clone = Instantiate(squaddie_prefab, squaddie_spawn_point, Quaternion.identity);
        player_squad_control.AddSquaddie(clone.GetComponent<Squaddie>());
    }


}
