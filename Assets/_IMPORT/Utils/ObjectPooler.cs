using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPooler<T> where T : MonoBehaviour
{
    //USE THIS https://www.gamasutra.com/blogs/SamIzzo/20180611/319671/Typesafe_object_pool_for_Unity.php

    [SerializeField] public List<T> pool;
    public T instance;
    public int poolSize;
    private int counter = 0;

    //Constructor | Give monobehavior script and pool size!
    public ObjectPooler(T instance, int amount)
    {
        if (instance == null)
            Debug.LogError("Instance is NULL!");


        Debug.Log("TEST");
        this.instance = instance;
        InitPool();
    }

    private void InitPool()
    {
        Debug.Log("1");
        pool = new List<T>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = UnityEngine.MonoBehaviour.Instantiate(instance.gameObject);
            obj.transform.gameObject.SetActive(false);
            pool.Add(obj.GetComponent<T>());
        }

        Debug.Log("2");
    }

    public T GetNext()
    {
        if (counter > poolSize) counter = 0;

        return pool[counter++];
    }

    public List<T> GetList()
    {
        return pool;
    }

}
