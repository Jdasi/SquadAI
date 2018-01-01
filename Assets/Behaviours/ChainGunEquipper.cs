using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChainGunEquipper : MonoBehaviour
{
    public ChainGun chain_gun { get; private set; }

    [Header("Parameters")]
    [SerializeField] Transform instantiate_parent;
    [SerializeField] Transform ray_cast_origin;
    [SerializeField] string instantiate_layer;
    [SerializeField] CustomEvents.ChainGunEvent instantiation_events;

    [Header("References")]
    [SerializeField] GameObject chain_gun_prefab;


    void Start()
    {
        var gun_clone = Instantiate(chain_gun_prefab, instantiate_parent.position,
            instantiate_parent.rotation);
        gun_clone.transform.SetParent(instantiate_parent);

        chain_gun = gun_clone.GetComponent<ChainGun>();
        chain_gun.Init(ray_cast_origin);

        // Mainly used to allow the player's gun to be rendered above all else in FPS view.
        int layer = instantiate_layer != "" ? LayerMask.NameToLayer(instantiate_layer) : 0;
        JHelper.SetLayerRecursive(gun_clone, layer);

        instantiation_events.Invoke(chain_gun);
    }

}
