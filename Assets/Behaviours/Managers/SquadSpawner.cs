using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnSettings
{
    public KeyCode spawn_key;
    public FactionSettings faction;

}

public class SquadSpawner : MonoBehaviour
{
    public List<SquaddieAI> all_units;

    [Header("References")]
    [SerializeField] GameObject squaddie_prefab;


    public SquadManager CreateSquad(FactionSettings _faction, int _num_squaddies, Vector3 _position)
    {
        SquadManager squad_manager = new SquadManager(_faction);

        for (int i = 0; i < _num_squaddies; ++i)
        {
            squad_manager.AddSquaddie(CreateSquaddie(_faction, _position));
        }

        return squad_manager;
    }


    public SquaddieAI CreateSquaddie(FactionSettings _settings, Vector3 _position)
    {
        GameObject clone = Instantiate(squaddie_prefab, _position, Quaternion.identity);
        SquaddieAI squaddie = clone.GetComponent<SquaddieAI>();

        squaddie.Init(_settings);

        all_units.Add(squaddie);
        return squaddie;
    }


    void Start()
    {

    }


    void Update()
    {
        all_units.RemoveAll(elem => elem == null);
    }

}
