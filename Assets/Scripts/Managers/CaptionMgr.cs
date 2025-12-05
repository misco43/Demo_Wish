using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// the CaptionMgr which manager all the caption show in the Game
/// </summary>
public class CaptionMgr : SingletoMono<CaptionMgr>
{
    public static float CAPTION_STAY_TIME = 1f;

    private CaptionMgr() { }

    [SerializeField] private List<string> _dialogList = new List<string>();
    [SerializeField] private int _dialogIndex = 0;

    /// <summary>
    /// show the caption which is sequene in this scene
    /// </summary>
    public void ShowCaption()
    {
        if(_dialogIndex < _dialogList.Count)
        {
            Debug.Log("Show the caption: ");
            UIManager.Instance.ShowPanel<CaptionPanel>().ShowCaption(_dialogList[_dialogIndex++], null);
        }
        else
        {
            Debug.LogWarning("All of the dialog has been show out!");
        }
    }
    
    /// <summary>
    /// Show the caption will been passed in 
    /// </summary>
    /// <param name="caption">the caption content</param>
    public Caption ShowCustomCaption(string caption, UnityAction callback = null)
    {
        return UIManager.Instance.ShowPanel<CaptionPanel>().ShowCaption(caption, callback);
    }
    

}
