using UnityEngine;
using System.Collections.Generic;

public class ItemDropContainer : MonoBehaviour
{
    public List<DropItemData> dropItemDatas = new List<DropItemData>();

    // 아이템을 드롭할 확률 리스트에서 고른다
    public DropItemData GetItemToDrop()
    {
        // 0~1 사이의 랜덤 값으로 드롭할 아이템 선택
        float totalChance = 0f;

        foreach (var itemData in dropItemDatas)
        {
            totalChance += itemData.dropChance;  // 전체 확률의 총합을 구한다
        }

        float randomValue = Random.Range(0f, totalChance);  // 랜덤 값 생성

        foreach (var itemData in dropItemDatas)
        {
            randomValue -= itemData.dropChance;

            if (randomValue <= 0f)
            {
                return itemData;  // 확률에 맞는 아이템을 반환
            }
        }

        return null;  // 아무것도 선택되지 않았다면 null을 반환
    }

    // 아이템을 드롭할 위치에서 생성
    public void GenerateItemAtPosition(Vector2 position, DropItemData itemToDrop)
    {
        // 아이템 생성
        Instantiate(itemToDrop.itemPrefab, position, Quaternion.identity);
    }
}
