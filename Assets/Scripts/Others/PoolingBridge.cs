using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PoolingBridge : MonoBehaviour, IPunPrefabPool
{
    private void Start()
    {
        PhotonNetwork.PrefabPool = this;
    }

    public void Destroy(GameObject gameObject)
    {
        //Instead of ACTUALLY destroying the gameObject, we will call isntead our Object Pool's Return
        ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        //Look for an instance of the object from the pool
        GameObject go = ObjectPoolManager.Instance.GetPooledObject(prefabId);
        go.transform.SetPositionAndRotation(position, rotation);
        return go;
    }
}
