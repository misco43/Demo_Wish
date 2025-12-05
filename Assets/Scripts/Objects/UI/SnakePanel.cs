using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnakePanel : BasePanel
{
    public RectTransform imgMainGameBK;
    public Button btnQuit;
    public TMP_Text txtCurScore;
    public TMP_Text txtMaxScore;

    [SerializeField] private GameObject _snakeGameTile;

    private void OnEnable()
    {
        base.Start();
        btnQuit.onClick.AddListener(() =>
        {
            HideMe(() =>
            {
                gameObject.SetActive(false);
            }); 
        });
    }

    public override void ShowMe()
    {
        gameObject.SetActive(true);
        base.ShowMe();
    }

    private void OnDisable()
    {
        btnQuit.onClick.RemoveAllListeners();
    }

    public void UpdateCurScore(string score)
    {
        txtCurScore.SetText($"得分：{score}");
    }

    public void UpdateMaxScore(string score)
    {
        txtMaxScore.SetText($"最高分：{score}");
    }
}
