using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the object which want to be interactable should achieve this inertface
/// </summary>
public interface IIntractable
{
    public abstract void EnterView();

    public abstract void Interact();

    public abstract void ExitView();
}
