using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This poolobject is a class that is used by the poolmanager
/// </summary>
public class PoolObject : MonoBehaviour {

    /// <summary>
    /// When the object is reused again after being 'destroyed'.
    /// </summary>
	public virtual void OnObjectReuse() {
        gameObject.SetActive(true);
	}
    /// <summary>
    /// On destroy, the object simply becomes inactive instead of destroyed.
    /// </summary>
	public virtual void Destroy(){
		gameObject.SetActive(false);
	}
}
