using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    private static Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private static Dictionary<string, GameObject> parentDictionary = new Dictionary<string, GameObject>();

    public static void PoolManagerInit()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();;
        parentDictionary = new Dictionary<string, GameObject>();
    }
    public static GameObject Instantiate(GameObject prefab)
    {
        return Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
    }
    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj;

        if(!parentDictionary.ContainsKey(prefab.name))
        {
            GameObject go = new GameObject(prefab.name);
            parentDictionary[prefab.name] = go;
            Debug.Log("생성해"+parentDictionary[prefab.name]);
        }

        if (poolDictionary.ContainsKey(prefab.name) && poolDictionary[prefab.name].Count > 0)
        {
            obj = poolDictionary[prefab.name].Dequeue();
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        else
        {
            obj = GameObject.Instantiate(prefab, position, rotation);
            obj.transform.parent = parentDictionary[prefab.name].transform;
            obj.name = prefab.name;
        }

        return obj;
    }

    public static void Destroy(GameObject obj)
    {
        obj.SetActive(false);

        if (!poolDictionary.ContainsKey(obj.name))
        {
            poolDictionary[obj.name] = new Queue<GameObject>();
        }

        poolDictionary[obj.name].Enqueue(obj);
    }
}
