using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the main enter of game
/// all event should be register in here to make sure the game can excute normally
/// </summary>
public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //register event
        //InteractObj event
        // EventCenter.Instance.AddEventListener<string>(E_EventType.InteractWithObj, (info) => {
        //     UIManager.Instance.ShowPanel<CaptionPanel>(UIManager.E_Layer.Top).ShowCaption(info);
        // });
    }
}
