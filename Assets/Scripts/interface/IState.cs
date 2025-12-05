using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
