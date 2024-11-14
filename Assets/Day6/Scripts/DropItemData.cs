using UnityEngine;

[CreateAssetMenu(fileName = "NewDropItem", menuName = "Item/DropItemData")]
public class DropItemData : ScriptableObject
{
    public GameObject itemPrefab;  // 아이템 프리팹
    public float dropChance;       // 드롭 확률 (0과 1 사이)
}