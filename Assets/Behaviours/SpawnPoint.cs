using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] FactionSettings faction;
    [SerializeField] int num_squaddies;
    [SerializeField] bool give_player_control;
    [SerializeField] bool spawn_on_start;


    public void SpawnSquaddies()
    {
        var squad = GameManager.scene.squad_spawner.CreateSquad(faction, num_squaddies, transform.position);

        if (give_player_control)
            GameManager.scene.player_squad_control.AddSquad(squad);
    }


    void Start()
    {
        if (spawn_on_start)
            SpawnSquaddies();
    }

}
