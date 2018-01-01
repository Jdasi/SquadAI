using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnSettings
{
    public KeyCode spawn_key;
    public FactionSettings faction;
}

/// <summary>
/// Centralised behaviour for constructing new squaddies.
/// References to squaddies are returned to classes that request them.
/// </summary>
public class SquadSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject squaddie_prefab;


    /// <summary>
    /// Creates a new squad with the passed faction settings.
    /// The squaddies are arranged in a circular pattern at the passed position, based on the squad size.
    /// </summary>
    /// <param name="_faction">The faction of the squad.</param>
    /// <param name="_num_squaddies">The size of the squad.</param>
    /// <param name="_pos">The where the squad should be arranged.</param>
    /// <returns>A reference to the instantiated squad.</returns>
    public SquadManager CreateSquad(FactionSettings _faction, int _num_squaddies, Vector3 _pos)
    {
        SquadManager squad_manager = new SquadManager(_faction);

        for (int i = 0; i < _num_squaddies; ++i)
        {
            SquaddieAI squaddie = CreateSquaddie(_faction, _pos);
            squaddie.transform.position = JHelper.PosToCirclePos(_pos, _num_squaddies, i, _num_squaddies * 0.5f);

            squad_manager.AddSquaddie(squaddie);
        }

        return squad_manager;
    }


    /// <summary>
    /// Constructs an individual squaddie with the passed faction settings.
    /// </summary>
    /// <param name="_faction">The faction of the squaddie.</param>
    /// <param name="_pos">The location the squaddie should be placed.</param>
    /// <returns>A reference to the instantiated squaddie.</returns>
    public SquaddieAI CreateSquaddie(FactionSettings _faction, Vector3 _pos)
    {
        GameObject clone = Instantiate(squaddie_prefab, _pos, Quaternion.identity);
        SquaddieAI squaddie = clone.GetComponent<SquaddieAI>();

        squaddie.Init(_faction);

        return squaddie;
    }

}
