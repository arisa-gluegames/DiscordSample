using System.Collections;
using UnityEngine;
using Photon.Pun;
public class EnemySpawner : MonoBehaviourPunCallbacks
{
    protected Boundary boundary;
    [SerializeField]
    //What is the id of the prefab we want to spawn from the pool
    protected string enemyId;
    [SerializeField]
    protected float minSpawnInterval = 2.0f;
    [SerializeField]
    protected float maxSpawnInterval = 5.0f;

    protected float spawnInterval;
    protected  bool isSpawning;

    protected virtual void Start()
    {
        CalculateScreenRestrictions();
    }

   
    protected virtual void Update()
    {
        if (!isSpawning)
            StartCoroutine(SpawnCoroutine());
    }

    protected void CalculateScreenRestrictions()
    {
        boundary = new Boundary();
        boundary.CalculateScreenRestrictions();
    }

   
    protected virtual IEnumerator SpawnCoroutine()
    {
        //Generate a random waiting interval
        spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        isSpawning = true;
        //Wait for the spawnInterval
        yield return new WaitForSeconds(spawnInterval);
        Spawn();
        isSpawning = false;
    }

    protected void Spawn()
    {
        SpawnEnemy();
    }

    protected virtual void SpawnEnemy()
    {
        //Get an object from the pool
        GameObject enemy = ObjectPoolManager.Instance.GetPooledObject(enemyId);
        //Did we get an object from the pool?
        if (enemy != null)
        {
            //Position the enemy 
            enemy.transform.position = GetSpawnPosition();
            enemy.SetActive(true);
        }
    }

    protected Vector2 GetSpawnPosition()
    {
        //Get a random vector based on the four edges of our screen
        int areaToSpawn = Random.Range(0, 4);
        switch (areaToSpawn)
        {
            //upper part - random x, fixed max y
            case 0:
                return new Vector2(Random.Range(-boundary.Bounds.x, boundary.Bounds.x),
                    boundary.Bounds.y);
            //lower part - random x, fixed min y
            case 1:
                return new Vector2(Random.Range(-boundary.Bounds.x, boundary.Bounds.x),
                   -boundary.Bounds.y);
            //right part - fixed max x, random y
            case 2:
                return new Vector2(boundary.Bounds.x, Random.Range(-boundary.Bounds.y,
                     boundary.Bounds.y));
            //left part - fixed min x, random y
            case 3:
                return new Vector2(-boundary.Bounds.x, Random.Range(-boundary.Bounds.y,
                     boundary.Bounds.y));
        }
        return Vector2.zero;
    }
}
