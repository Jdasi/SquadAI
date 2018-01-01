using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneArranger : MonoBehaviour
{
    [SerializeField] List<HackableConsole> consoles_back_row;
    [SerializeField] List<HackableConsole> consoles_middle_row;
    [SerializeField] List<HackableConsole> consoles_front_row;

    [Space]
    [SerializeField] List<SpawnPoint> enemy_spawns;

    public List<HackableConsole> active_consoles = new List<HackableConsole>();


    void Start()
    {
        EnableRandomConsole(consoles_back_row);
        EnableRandomConsole(consoles_middle_row);
        EnableRandomConsole(consoles_front_row);

        SpawnBadGuys();
    }


    void EnableRandomConsole(List<HackableConsole> _consoles)
    {
        int index = Random.Range(0, _consoles.Count);

        for (int i = 0; i < _consoles.Count; ++i)
        {
            var console_obj = _consoles[i].gameObject;
            console_obj.SetActive(i == index ? true : false);

            if (i == index)
                active_consoles.Add(_consoles[i]);
        }
    }


    void SpawnBadGuys()
    {
        int spawn_counter = 0;

        while (spawn_counter < GameManager.squads_to_spawn)
        {
            if (spawn_counter >= enemy_spawns.Count)
                break;

            SpawnPoint spawn = enemy_spawns[Random.Range(0, enemy_spawns.Count)];
            spawn.SpawnSquaddies();

            enemy_spawns.Remove(spawn);

            ++spawn_counter;
        }
    }

}
