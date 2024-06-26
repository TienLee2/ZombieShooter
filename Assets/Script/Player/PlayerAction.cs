using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    //The current gun player use
    private GunSelector GunSelector;
    //Player input, using the new input system
    private StarterAssetsInputs _input;

    [SerializeField]
    private bool AutorReload = true;
    [SerializeField]
    private Animator PlayerAnimator;
    [SerializeField]
    private PlayerIK InverseKinematic;

    private bool IsReloading;
    private bool isHolding = false;

    private void Start()
    {
        //Get some reference
        GunSelector = GetComponent<GunSelector>();
        _input = GetComponent<StarterAssetsInputs>();
        PlayerAnimator = GetComponent<Animator>();
        InverseKinematic = GetComponent<PlayerIK>();
    }

    private void Update()
    {
        ShootWithRaycast();
        ShootWithRigidbody();

        //Check if player press reload button or guns out of ammo
        if (ShouldManualReload() || ShouldAutoReload())
        {
            IsReloading = true;
            PlayerAnimator.SetTrigger("Reload");
            InverseKinematic.HandIKAmount = 0.25f;
            InverseKinematic.ElbowIKAmount = 0.25f;
            _input.reload = false;
        }
    }

    //Shoot with raycast, press or hold the left mouse button to shoot
    private void ShootWithRaycast()
    {
        if (Input.GetMouseButton(0) && GunSelector.ActiveGun != null && GunSelector.ActiveGun.Type != GunType.Bazooka)
        {
            GunSelector.ActiveGun.Shoot();
            Debug.Log("Shoot");
        }
    }

    //Shoot with rigidbody gun aka bazooka, must hold and release to shoot
    private void ShootWithRigidbody()
    {
        if (Input.GetMouseButton(0) && GunSelector.ActiveGun.Type == GunType.Bazooka)
        {
            isHolding = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isHolding)
            {
                GunSelector.ActiveGun.Shoot();
                Debug.Log("Shoot");
                isHolding = false;
            }
        }
    }


    private void EndReload()
    {
        GunSelector.ActiveGun.EndReload();
        InverseKinematic.HandIKAmount = 1f;
        InverseKinematic.ElbowIKAmount = 1f;
        IsReloading = false;

    }

    private bool ShouldManualReload()
    {
        return _input.reload
            && !IsReloading
            && GunSelector.ActiveGun.CanReload();
    }

    private bool ShouldAutoReload()
    {
        return AutorReload
            && !IsReloading
            && GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo == 0
            && GunSelector.ActiveGun != null;
    }
}
