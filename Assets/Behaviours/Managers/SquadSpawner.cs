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

    [Header("Parameters")]
    [SerializeField] SpawnSettings friendly_settings;
    [SerializeField] SpawnSettings enemy_settings;

    [Space]
    [SerializeField] KeyCode remove_squaddie_key = KeyCode.LeftBracket;

    [Header("References")]
    [SerializeField] SquadControl player_squad_control;
    [SerializeField] GameObject squaddie_prefab;


    void Start()
    {

    }


    void Update()
    {
        all_units.RemoveAll(elem => elem == null);

        if (Input.GetKeyDown(friendly_settings.spawn_key))
        {
            SpawnSquaddie(friendly_settings.faction);
        }
        else if (Input.GetKeyDown(enemy_settings.spawn_key))
        {
            SpawnSquaddie(enemy_settings.faction);
        }

        if (Input.GetKeyDown(remove_squaddie_key))
        {
            if (player_squad_control.issuing_order)
                player_squad_control.RemoveSquaddie();
        }
    }


    void SpawnSquaddie(FactionSettings _settings)
    {
        ContextType current_context_type = GameManager.scene.context_scanner.current_context.type;
        Vector3 indicator_position = GameManager.scene.context_scanner.current_context.indicator_position;

        if (!player_squad_control.issuing_order || current_context_type != ContextType.FLOOR)
            return;

        GameObject clone = Instantiate(squaddie_prefab, indicator_position, Quaternion.identity);

        clone.GetComponent<SquaddieStats>().faction_settings = _settings;

        SquaddieAI squaddie = clone.GetComponent<SquaddieAI>();
        squaddie.Init();

        all_units.Add(squaddie);
        player_squad_control.AddSquaddie(squaddie);
    }


}
