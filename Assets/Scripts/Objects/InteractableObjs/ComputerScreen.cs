using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerScreen : MonoBehaviour
{
    public Button btnSnake;
    public Button btnPaperView;
    public Button btnUnknown;
    
    
    public SnakePanel snakePanel; 
    private bool _hasClickUnknown;


    private void OnEnable()
    {
        //show the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        btnSnake.onClick.AddListener(() =>
        {
           print("Open the snake gamePanel"); 
           snakePanel.ShowMe();
        });

        btnPaperView.onClick.AddListener(() =>
        {
            print("Open the parperView panel");
        });

        btnUnknown.onClick.AddListener(() =>
        {
            if (!_hasClickUnknown)
            {
                _hasClickUnknown = true;
                CaptionMgr.Instance.ShowCustomCaption("这个应用是什么？？，点击了也没有反应", () =>
                {
                    //TODO:Flash bright white light
                    print("Flash the white light, this scene is over");
                });
            }
        });
    }

    private void OnDisable()
    {
        //hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        btnSnake.onClick.RemoveAllListeners();
        btnPaperView.onClick.RemoveAllListeners();
        btnUnknown.onClick.RemoveAllListeners();
    }
}
