using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyObject;
    public Transform[] spawnPoints;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.fixedDeltaTime;
        if (timer > 5f)
        {
            MonsterSpawn();
            timer = 0;
        }
    }
    void MonsterSpawn()
    {
        GameObject enemy = Instantiate(enemyObject);
        enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
    }
    
}
