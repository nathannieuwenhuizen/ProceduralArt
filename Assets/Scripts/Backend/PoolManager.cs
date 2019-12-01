using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The poolmanager is generic and handles the object pooling and reusing of reusable prefabs.
/// </summary>
public class PoolManager : MonoBehaviour {

    //Creates a list of object instances.
	Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();


    //An instance making sure only one of this object can exist in a scene or application? I dunno
	static PoolManager _instance;
	public static PoolManager instance {
		get{
			if (_instance == null) {
				_instance = FindObjectOfType<PoolManager>();
			} 
			return _instance;
		}
	}

    /// <summary>
    /// Creates the pool of the prefabs on how big it should be, it loads them all immediatly
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="size"></param>
	public void CreatePool(GameObject prefab, int size) {
		int key = prefab.GetInstanceID ();

		if (!poolDictionary.ContainsKey(key)) {

            GameObject group = new GameObject(prefab.name + " pool");
            group.transform.parent = transform;

            poolDictionary.Add(key, new Queue<ObjectInstance>());

			for (int i = 0; i < size; i ++) {
                CreateInstanceInPool(key, prefab, group);
			}
		}
	}
    public ObjectInstance CreateInstanceInPool(int key, GameObject prefab, GameObject group)
    {
        ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject);
        poolDictionary[key].Enqueue(newObject);
        newObject.SetParent(group.transform);
        return newObject;
    }

    /// <summary>
    /// Reuses the prefab from the object pool back into the field
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
	public GameObject ReuseObject(GameObject prefab, Vector3 pos, Quaternion rot) {
		int key = prefab.GetInstanceID();
 
		if (poolDictionary.ContainsKey(key)) {
			ObjectInstance obj = poolDictionary[key].Dequeue();
			poolDictionary[key].Enqueue(obj);
            if (obj.gameObject.activeSelf)
            {
                //if pool is full, it adds a new object to the pool
                GameObject parent = obj.gameObject.transform.parent.gameObject;
                obj = CreateInstanceInPool(key, prefab, parent);
            }
			obj.Reuse(pos, rot);
			return obj.gameObject;
		}
		return null;
	}

    /// <summary>
    /// The object instance is a gameobject that is held in the pool class, it is given a certain name and is made inactive in the group of the pool.
    /// </summary>
	public class ObjectInstance {
        //the gameobject itself
		public GameObject gameObject;
        //its position
		Transform transform;
        //checks if it has pool object component in itself, prevents bugs.
		bool hasPoolComponent;
        //the poolobject script itself.
		PoolObject poolObjectScript;

        /// <summary>
        /// Creates the pool object and defines the variables
        /// Then it is placed into the pool group (this gameobject)
        /// </summary>
        /// <param name="objectInstance"></param>
		public ObjectInstance(GameObject objectInstance) {
			gameObject = objectInstance;
			transform = gameObject.transform;
			gameObject.SetActive(false);
			if (gameObject.GetComponent<PoolObject>()) {
				poolObjectScript = gameObject.GetComponent<PoolObject>();
			}
		}
        /// <summary>
        /// Reuses the object, placing it outside of the pool group and makes it active.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
		public void Reuse(Vector3 pos, Quaternion rot) {
			if (gameObject.GetComponent<PoolObject>()) {
				poolObjectScript.OnObjectReuse();
			}
			gameObject.SetActive(true);
			transform.position = pos;
			transform.rotation = rot;
		}
        /// <summary>
        /// Makes the transofrm the parent of this gameobject.
        /// </summary>
        /// <param name="parent"></param>
		public void SetParent(Transform parent){
			transform.parent = parent;
		}
	}
}
