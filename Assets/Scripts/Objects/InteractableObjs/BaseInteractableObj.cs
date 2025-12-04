using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteractableObj : MonoBehaviour, IIntractable
{
    public bool isInteracting;  //be interacting nowatime

    public virtual void EnterView()
    {
        isInteracting = true;
    }

    public virtual void Interact()
    {

    }

    public virtual void ExitView()
    {
        isInteracting = false;
    }
}
