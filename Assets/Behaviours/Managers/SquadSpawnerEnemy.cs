using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSpawnerEnemy : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] KeyCode spawn_key = KeyCode.P;
    [SerializeField] FactionSettings spawn_faction;
    [SerializeField] Vector3 enemy_waypoint;

    [Header("References")]
    [SerializeField] GameObject squaddie_prefab;
    [SerializeField] List<Transform> spawn_points;


    void Update()
    {
        if (Input.GetKeyDown(spawn_key))
            SpawnEnemy();
    }


    void SpawnEnemy()
    {
        Vector3 random_spawn = spawn_points[Random.Range(0, spawn_points.Count)].position;
        GameObject clone = Instantiate(squaddie_prefab, random_spawn, Quaternion.identity);

        clone.GetComponent<SquaddieStats>().faction_settings = spawn_faction;

        var agent = clone.GetComponent<SquaddieAI>();
        agent.Init();

        agent.IssueMoveCommand(enemy_waypoint);
    }

}
