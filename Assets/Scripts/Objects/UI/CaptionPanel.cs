using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CaptionPanel : BasePanel
{
    [SerializeField] private GameObject _captionPrefab;
    private Caption _playerCation;

    /// <summary>
    /// Show the caption with the paramenter pass in
    /// </summary>
    /// <param name="content">the caption content you want to show </param>
    public Caption ShowCaption(string content, UnityAction callback)
    {
        //TODO: To deal the question of when caption is playing and other place call to show a new caption
        if(_playerCation == null)
        {
            _playerCation = Instantiate<GameObject>(_captionPrefab, transform).GetComponent<Caption>();
            _playerCation.ShowText(content);
            _playerCation.OnCaptionEnd += callback;

            return _playerCation;
        }
        else if(_playerCation.State == CaptionState.Hidden)
        {
            //playerCation.Init(content);
            _playerCation.ShowText(content);
            _playerCation.OnCaptionEnd += callback;

            return _playerCation; 
        }
        else
        {
            //Init a new Caption obj to show
            Caption caption = Instantiate<GameObject>(_captionPrefab, transform).GetComponent<Caption>();
            caption.targetText.SetText(content);
            caption.ShowText(content);

             _playerCation.OnCaptionEnd += callback;
            caption.OnCaptionEnd += () =>
            {
               Destroy(caption.gameObject);  
            };

            return caption;
        }

    }
}
