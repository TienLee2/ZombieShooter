using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    //Get the enemy gameobject to spawn
    public GameObject objectToSpawn; 
    //The area to spawn gameobject in
    public BoxCollider spawnArea; 
    //number of gameobject to spawn
    public int numberOfObjectsToSpawn = 10; 
    //bool to generate key to process to next level
    public bool chosenRoom;
    //The key 
    public GameObject KeyToNextLevel;
    //The area to check if player enter the room yet
    public BoxCollider entryCollider;

    //A list of spawn enemy
    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private Transform player;
    private bool Alert;

    void Start()
    {
        var play = GameObject.FindGameObjectWithTag("Player");
        player = play.transform;
        SpawnObject();
        //If this is the chosen room, then spawn the key
        if (chosenRoom)
        {
            Vector3 centerPosition = spawnArea.bounds.center;
            Lean.Pool.LeanPool.Spawn(KeyToNextLevel, centerPosition, Quaternion.identity);
        }
    }

    private void Update()
    {
        //This method is for optimization, check the document for more information
        for(int i =0; i < spawnedEnemies.Count; i++)
        {
            if (Alert)
            {
                spawnedEnemies[i].player = player;
            }
            spawnedEnemies[i].UpdateMe();
        }
    }

    void SpawnObject()
    {
        // Get the bounds of the spawn area
        Bounds bounds = spawnArea.bounds;

        for(int i =0; i < numberOfObjectsToSpawn; i++)
        {
            // Generate random positions within the bounds
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            // Create a random position
            Vector3 randomPosition = new Vector3(x, y, z);

            // Instantiate the GameObject at the random position
            GameObject Enemy = Lean.Pool.LeanPool.Spawn(objectToSpawn, randomPosition, Quaternion.identity);
            // Get the Enemy script from the spawned object
            Enemy enemyScript = Enemy.GetComponent<Enemy>();

            // Add the Enemy script to the list
            if (enemyScript != null)
            {
                spawnedEnemies.Add(enemyScript);
            }
            else
            {
                Debug.LogWarning("Spawned object does not have an Enemy script attached.");
            }
        }
        
    }

    public void AlertZombie()
    {
        Alert = true;
    }

}
