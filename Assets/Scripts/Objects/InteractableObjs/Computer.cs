using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : BaseInteractableObj
{
    public Renderer render;
    public Material originMat;
    public Material interactMat;

    public override void EnterView()
    {
        if (isInteracting) return;

        base.EnterView();
        render.sharedMaterial = interactMat;
    }

    public override void Interact()
    {
        base.Interact();
        // EventCenter.Instance.EventTrigger<string>(E_EventType.InteractWithObj, "这是我的电脑，我每天在它面前度过我无聊的人生");
        Caption caption = CaptionMgr.Instance.ShowCustomCaption("这是我的电脑，我每天在它面前度过我无聊的人生", TimeLineMgr.Instance.OnPlay);
    }

    public override void ExitView()
    {
        if (!isInteracting) return;

        base.ExitView();

        render.sharedMaterial = originMat;
    }
}
