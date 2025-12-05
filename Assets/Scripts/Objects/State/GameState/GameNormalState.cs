using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNormalState : MonoBehaviour, IState
{
    public void OnEnter()
    {
        //set the controll power back to the player
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        //canel the player's controll power
    }

}
