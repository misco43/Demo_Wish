using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Singleton<T> where T : class 
{
    protected bool InstancIsNull => instance == null;
    protected static readonly object locak = new object();

    private static T instance;
    public static T Instance
    {
        get {
            if (instance == null) {
                //加锁
                lock (locak) {
                    if (instance == null) {
                        //获取传入对象中的私有构造函数
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogWarning(type.Name + "没有对应的私有无参构造函数");

                    }
                }
            }
            
            return instance;
        }
    }


}
