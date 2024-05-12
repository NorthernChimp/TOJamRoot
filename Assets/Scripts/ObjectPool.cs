using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPool : MonoBehaviour
{
    // Start is called before the first frame update
    public static ObjectPool instance;
    public static List<PooledObjectInfo> objectPools = new List<PooledObjectInfo>();
    void Start()
    {
        
    }
    public static GameObject SpawnObject(GameObject objectToSpawn,Vector3 spawnPosition,Quaternion spawnRotation)
    {
        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == objectToSpawn.name);
        if (pool == null)
        {
            pool = new PooledObjectInfo("poo") { }; 
            objectPools.Add(pool);
        }
        GameObject spawnableObject = pool.inactiveObjects.FirstOrDefault();

        if(spawnableObject == null)
        {
            spawnableObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
        }
        else
        {
            spawnableObject.transform.position = spawnPosition;
            spawnableObject.transform.rotation = spawnRotation;
            pool.inactiveObjects.Remove(spawnableObject);
            spawnableObject.SetActive(true);
        }
        return spawnableObject;
    }
    public static GameObject SpawnObjectFromName(string objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == objectToSpawn);
        if (pool == null)
        {
            pool = new PooledObjectInfo(objectToSpawn);
            objectPools.Add(pool);
        }
        GameObject spawnableObject = pool.inactiveObjects.FirstOrDefault();

        if (spawnableObject == null)
        {
            spawnableObject = Instantiate(pool.GetPrefab(), spawnPosition, spawnRotation);
        }
        else
        {
            spawnableObject.transform.position = spawnPosition;
            spawnableObject.transform.rotation = spawnRotation;
            pool.inactiveObjects.Remove(spawnableObject);
            spawnableObject.SetActive(true);
        }
        return spawnableObject;
    }
    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7);
        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == goName);
        if(pool == null)
        {
            Debug.LogWarning("Trying to remove an object without a pool " + obj.name);
        }
        else
        {
            obj.SetActive(false);
            pool.inactiveObjects.Add(obj);
        }
    }
    //public GameObject
    private void Awake()
    {
        if (!instance) { instance = this; }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
public class PooledObjectInfo
{
    public string lookupString;
    public List<GameObject> inactiveObjects = new List<GameObject>();
    GameObject prefab; public GameObject GetPrefab() { return prefab; }
    public PooledObjectInfo(string prefName) { prefab = Resources.Load(prefName) as GameObject;lookupString = prefName; }
}