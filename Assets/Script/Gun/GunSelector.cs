using Sirenix.OdinInspector;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent]
public class GunSelector : MonoBehaviour
{
    //This gun system can use for both player and enemy
    //So there must be some boolean to check and get reference
    public bool IsThisPlayer;
    [SerializeField]
    private GunType Gun;
    //The current paretn gameobject that hold the gun
    [SerializeField]
    private Transform GunParent;
    //List of guns
    [SerializeField]
    private List<GunScriptableObject> Guns;
    [SerializeField]
    private PlayerIK InverseKinematics;

    [Space]
    [Header("Runtime Filled")]
    public GunScriptableObject ActiveGun;


    private ThirdPersonController ThirdPersonController;
    private float OriginalMoveSpeed;
    private float OriginalRunSpeed;

    private void Start()
    {
        if (IsThisPlayer)
        {
            ThirdPersonController = GetComponent<ThirdPersonController>();
            OriginalMoveSpeed = ThirdPersonController.MoveSpeed;
            OriginalRunSpeed = ThirdPersonController.SprintSpeed;
        }

        //Find the gun in guns list
        GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun);
        if (gun == null)
        {
            Debug.Log("NO Gun Scriptable object can found for: " + gun);
            return;
        }
        //then set it to be the active gun and spawn it
        ActiveGun = gun;
        gun.Spawn(GunParent, this);

        //handle IK
        Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
        InverseKinematics.LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        InverseKinematics.RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        InverseKinematics.LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        InverseKinematics.RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");

    }

    private void Update()
    {
        //Control the movement of guns
        if (IsThisPlayer)
        {
            switch (ActiveGun.Type)
            {
                case GunType.HandGun:
                    ThirdPersonController.MoveSpeed = OriginalMoveSpeed;
                    break;
                case GunType.AssaultRifle:
                    ThirdPersonController.MoveSpeed = OriginalMoveSpeed * 0.75f;
                    ThirdPersonController.SprintSpeed = OriginalRunSpeed * 0.75f;
                    break;
                case GunType.Bazooka:
                    ThirdPersonController.MoveSpeed = OriginalMoveSpeed * 0.55f;
                    ThirdPersonController.SprintSpeed = OriginalRunSpeed * 0.55f;
                    break;
                default:
                    ThirdPersonController.MoveSpeed = OriginalMoveSpeed;
                    break;

            }
        }

    }
}
