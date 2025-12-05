using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;


public class ABResMgr : Singleton<ABResMgr>
{
    private ABResMgr(){}

    private bool _isDebug = true;   //debug model means that res load from the editor

    /// <summary>
    /// Can Load Res:
    /// 1.GameObject
    /// 2.Texture
    /// 3.Matrial
    /// 4.AudioClip
    /// 5.VideoClip
    /// 6.Atlas
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callback"></param>
    /// <param name="isSync"></param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callback,  bool isSync = false) where T : Object
    {
#if UNITY_EDITOR
        if (_isDebug)
        {
            //load by editorMgr
            Debug.Log($"Load Res From Editor Mode, , AB Name is [{abName}], res Name is [{resName}]" );
            T res = EditorResMgr.Instance.LoadEditorRes<T>($"{abName}/{resName}");
            callback?.Invoke(res as T);                
        }
        else
        {
            //load by ABMgr
            Debug.Log($"Load Res From AB Mode, , AB Name is [{abName}], res Name is [{resName}]" );
            ABMgr.Instance.LoadResAsync<T>(abName, resName, callback, isSync);
        }
#else
        //load by ABMgr
        ABMgr.Instance.LoadResAsync<T>(abName, resName, callback, isSync);
#endif
    }
}

public static class ABName
{
    public readonly static string UI_ITEM = "prefabs/ui";
}
