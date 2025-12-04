using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgr : Singleton<PoolMgr>
{
    private PoolMgr() { }

    private Dictionary<string, Stack<GameObject>> poolDic = new Dictionary<string, Stack<GameObject>>();

    public GameObject Get<T>() where T : MonoBehaviour
    {
        string name = typeof(T).Name;
        GameObject obj;
        if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        {
            obj = poolDic[name].Pop();
            obj.SetActive(true); 
        }
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
        }

        return obj;
    }

    public void Push<T>(T obj) where T : MonoBehaviour
    {
        string name = typeof(T).Name;
        if (!poolDic.ContainsKey(name))
            poolDic[name] = new Stack<GameObject>();

        obj.gameObject.SetActive(false);
        poolDic[name].Push(obj.gameObject);
    }

    public void Clear()
    {
        poolDic.Clear();
    }

}
