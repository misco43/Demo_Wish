using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : SingletonAutoMono<MonoMgr>
{
    public UnityAction onUpdate;
    public UnityAction onFixUpdate;
    public UnityAction onLateUpdate;


    private void Update()
    {
        onUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        onFixUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        onLateUpdate?.Invoke();
    }
}

