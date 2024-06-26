using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyToNextLevel : MonoBehaviour
{
    //Load next scene if player collect the key
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.instance.LoadNextLevel();

            Destroy(gameObject);
        }
    }
}
