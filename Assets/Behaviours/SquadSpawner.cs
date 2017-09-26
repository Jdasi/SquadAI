using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSpawner : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] KeyCode spawn_squaddie_key = KeyCode.RightBracket;
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
    }


    void SpawnSquaddie()
    {
        GameObject clone = Instantiate(squaddie_prefab, squaddie_spawn_point, Quaternion.identity);
        player_squad_control.AddSquaddie(clone.GetComponent<Squaddie>());
    }


}
