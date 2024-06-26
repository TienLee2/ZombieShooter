using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    public List<RandomSpawner> spawner = new List<RandomSpawner>();
    [SerializeField]private int chosenKeyRoomNumber;

    private void Awake()
    {
        
        chosenKeyRoomNumber = Random.Range(0,spawner.Count);
        spawner[chosenKeyRoomNumber].chosenRoom = true;
    }
}
