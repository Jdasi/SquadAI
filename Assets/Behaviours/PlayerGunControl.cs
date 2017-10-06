using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunControl : MonoBehaviour
{
    private ChainGun chain_gun;
    private bool can_shoot;


    public void EnableShooting()
    {
        can_shoot = true;
    }


    public void DisableShooting()
    {
        can_shoot = false;
    }


    public void SetChainGun(ChainGun _gun)
    {
        chain_gun = _gun;
    }


    public void Update()
    {
        if (chain_gun != null && can_shoot)
            chain_gun.cycle = Input.GetMouseButton(0);
    }

}
