using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    private PlayerGunSelector GunSelector;
    private StarterAssetsInputs _input;

    private void Start()
    {
        GunSelector = GetComponent<PlayerGunSelector>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if(_input.shoot && GunSelector.ActiveGun != null)
        {
            GunSelector.ActiveGun.Shoot();
            Debug.Log("Shoot");
        }
    }
}
