using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public List<GameObject> itemPrefabsList;
    List<GameObject> generatedItemList = new List<GameObject>();
    [SerializeField] private List<Vector2> spawnedPositions = new List<Vector2>();

    public Transform player; // 플레이어 위치

    public int itemCount = 10;
    public float maxSpawnRange = 50f;
    public float minSpawnRange = 5f;
    public float itemRadius = 1f;
    public float minDistanceFromPlayer = 5f; // 아이템과 플레이어 사이 최소 거리

    private void Start()
    {
        GameManager.Instance.GameStart += SpawnItems;
    }

    void SpawnItems()
    {
        int spawned = 0;
        int maxAttempts = 1000;
        int attempts = 0;

        while (spawned < itemCount && attempts < maxAttempts)
        {
            var index = Random.Range(0, itemPrefabsList.Count);
            float spawnDistance = Random.Range(minSpawnRange, maxSpawnRange);

            var range = Random.insideUnitCircle;

            var randomPosition = range * spawnDistance;


            // 플레이어와 겹치지 않게 위치 확인
            if (IsPositionAvailable(randomPosition))
            {
                var itemGameObject = PoolingManager.Instantiate(itemPrefabsList[index], randomPosition, Quaternion.identity);
                itemGameObject.GetComponent<Item>().Init();
                generatedItemList.Add(itemGameObject);
                spawnedPositions.Add(randomPosition);
                spawned++;
            }

            attempts++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("최대 시도 횟수에 도달하여 일부 아이템이 생성되지 않았습니다.");
        }
    }

    bool IsPositionAvailable(Vector2 newPosition)
    {
        // 플레이어와의 최소 거리 체크
        if (Vector2.Distance(newPosition, player.position) < minDistanceFromPlayer)
        {
            return false;
        }

        // 다른 아이템들과의 거리 체크
        foreach (Vector2 position in spawnedPositions)
        {
            if (Vector2.Distance(position, newPosition) < itemRadius * 2)
            {
                return false;
            }
        }
        return true;
    }
}