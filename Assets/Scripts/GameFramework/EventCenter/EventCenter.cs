using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public abstract class EventInfoBase{ }


public class EventInfo<T> : EventInfoBase
{
    public UnityAction<T> actions;

    public EventInfo (UnityAction<T> func){
        actions += func;
    }
}


public class EventInfo : EventInfoBase
{
    public UnityAction actions;

    public EventInfo(UnityAction func)
    {
        actions += func;
    }
}

public enum E_EventType
{
    InteractWithObj,
    Test,
}


public class EventCenter : Singleton<EventCenter>
{
    private EventCenter() { }

    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();

    public void EventTrigger<T>(E_EventType eventType, T info)
    {
        Debug.Log("Event center Trigger Event " + eventType.ToString());
        if (eventDic.ContainsKey(eventType)) {
            Debug.Log((eventDic[eventType] as EventInfo<T>).actions.ToString());
            (eventDic[eventType] as EventInfo<T>).actions?.Invoke(info);
        }
    }


    public void EventTrigger(E_EventType eventType)
    {
        Debug.Log("Event center Trigger Event " + eventType.ToString());
        if (eventDic.ContainsKey(eventType)) {
            Debug.Log((eventDic[eventType] as EventInfo).actions.ToString());
            (eventDic[eventType] as EventInfo).actions?.Invoke();
        }
    }


    public void AddEventListener<T>(E_EventType eventType, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventType)) {
            (eventDic[eventType] as EventInfo<T>).actions += func;
        }
        else {
            eventDic.Add(eventType, new EventInfo<T>(func));
        }
    }

    public void AddEventListener(E_EventType eventType, UnityAction func)
    {
        if (eventDic.ContainsKey(eventType)) {
            (eventDic[eventType] as EventInfo).actions += func;
        }
        else {
            eventDic.Add(eventType, new EventInfo(func));
        }
    }


    public void RemoveEventListener<T>(E_EventType eventType, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventType)) {
            (eventDic[eventType] as EventInfo<T>).actions -= func;
        }
    }


    public void RemoveEventListener(E_EventType eventType, UnityAction func)
    {
        if (eventDic.ContainsKey(eventType)) {
            (eventDic[eventType] as EventInfo).actions -= func;
        }
    }


    public void Clear()
    {
        eventDic.Clear();
    }

    public void Clear(E_EventType eventType)
    {
        if (eventDic.ContainsKey(eventType))
            eventDic.Remove(eventType);
    }
}
