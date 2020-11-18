using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public Button Prefab;
        public int Size;
    }

    public static ButtonPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Pool> pools;

    public Dictionary<string, Queue<Button>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<Button>>();

        foreach (Pool pool in pools)
        {
            Queue<Button> objectPool = new Queue<Button>();

            for(int i = 0; i < pool.Size; i++)
            {
                Button obj = Instantiate(pool.Prefab);
                obj.gameObject.SetActive(false);
                objectPool.Enqueue(obj);
                obj.transform.SetParent(transform, false);
            }

            poolDictionary.Add(pool.Tag, objectPool);
        }
    }

    public Button SpawnFromPool(string tag, Vector3 position, Quaternion rotation, bool active, string text)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        Button objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.gameObject.SetActive(active);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.GetComponentInChildren<Text>().text = text;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
