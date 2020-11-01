using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour{

    [System.Serializable]
    public class Pool{
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public static ObjectPooler instance;

    void Awake(){
        instance = this;

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools){
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++){
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.parent = transform;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary> Spawns a GameObject taken from a pool to a specific position in world space. </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position){
        if (!poolDictionary.ContainsKey(tag)){
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = Quaternion.identity;
        if (objectToSpawn.transform.parent != transform){
            objectToSpawn.transform.SetParent(transform);
        }

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null){
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    /// <summary> Spawns a GameObject taken from a pool to a specific position and rotation in world space. </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation){
        if (!poolDictionary.ContainsKey(tag)){
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        if (objectToSpawn.transform.parent != transform){
            objectToSpawn.transform.SetParent(transform);
        }

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null){
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    /// <summary> Spawns a GameObject taken from a pool to a specific position and rotation in world space and sets it as a child of a transform. </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent){
        if (!poolDictionary.ContainsKey(tag)){
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        if (parent != null){
            objectToSpawn.transform.SetParent(parent);
        }

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null){
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    /// <summary> Spawns a GameObject taken from a pool to a specific position and rotation in world or local space and sets it as a child of a transform. </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent, bool instantiateInWorldSpace){
        if (!poolDictionary.ContainsKey(tag)){
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        if (instantiateInWorldSpace){
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            if (parent != null){
                objectToSpawn.transform.SetParent(parent);
            }
        }else if(parent != null){
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.SetParent(parent);
            objectToSpawn.transform.localPosition = position;
            objectToSpawn.transform.localRotation = rotation;
        }else{
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            if (parent != null){
                objectToSpawn.transform.SetParent(parent);
            }
        }

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null){
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
