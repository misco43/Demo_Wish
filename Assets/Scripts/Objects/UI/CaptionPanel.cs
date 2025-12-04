using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CaptionPanel : BasePanel
{
    public GameObject playCationPrefab;
    private PlayerTxtAnimation playerCation;

    protected override void Start()
    {
        base.Start();
        if(playerCation == null)
            playCationPrefab = Resources.Load<GameObject>("Prefabs/UI/Items/txtPlayerCaption");
    }

    public void ShowCaption(string caption)
    {
        playerCation = Instantiate<GameObject>(playCationPrefab, transform).GetComponent<PlayerTxtAnimation>();
        playerCation.targetText.text = caption;
        playerCation.ShowText();

        Invoke("HideText", 3f);
    }

    private void HideText()
    {
        playerCation.HideText();
    }
}
