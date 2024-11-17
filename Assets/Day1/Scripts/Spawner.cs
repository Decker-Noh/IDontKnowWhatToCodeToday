using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    public Transform[] spawnPoints;
    float timer;
    float spawnIntervalTime = 2f;
    [SerializeField] float accumulatedTime;

    float levelUpInterval = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.gameStateEnum != GameStateEnum.InGame) return;
        timer += Time.fixedDeltaTime;
        accumulatedTime += Time.fixedDeltaTime;
        GameManager.Instance.GameTime = accumulatedTime;
        if (timer > spawnIntervalTime)
        {
            MonsterSpawn();
            timer = 0;
        }
    }
    void MonsterSpawn()
    {
        GameObject enemyObject = PoolingManager.Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Length)]);
        enemyObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        var enemy = enemyObject.GetComponent<Enemy>();

        //0.01f = 100초 지나면 1레벨 오름
        //0.04f = 25초 지나면
        //0.1f = 10초 지나면


        var spawnMaxlevel = Mathf.RoundToInt(accumulatedTime * levelUpInterval);

        GameManager.Instance.StageLevel = spawnMaxlevel;

        enemy.Level = GetWeightedLevel(spawnMaxlevel);

        if(enemy.enemyKind == EnemyKind.TrackingEnemy) enemy.Level = enemy.Level * 100;


    }

    int GetWeightedLevel(int maxLevel)
    {
        float[] probabilities = new float[maxLevel];
        float totalWeight = 0f;

        // 각 레벨에 가중치를 부여해 낮은 레벨에 높은 확률 부여 (1부터 시작)
        for (int i = 1; i <= maxLevel; i++)
        {
            float weight = 1f / i; // 낮은 레벨일수록 높은 가중치
            probabilities[i - 1] = weight;
            totalWeight += weight;
        }

        // 0~1 사이에서 무작위 값을 생성하여 확률을 기반으로 레벨 선택
        float randomValue = Random.value * totalWeight;
        float cumulativeWeight = 0f;

        for (int i = 1; i <= maxLevel; i++)
        {
            cumulativeWeight += probabilities[i - 1];
            if (randomValue < cumulativeWeight)
            {
                return i; // 선택된 레벨 반환
            }
        }

        return 1; // 만약 확률 계산이 벗어나면 최소 레벨 반환
    }

}
