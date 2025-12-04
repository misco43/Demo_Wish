using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Load Res From the AB package
/// </summary>
public class ABMgr : SingletonAutoMono<ABMgr>
{
    private AssetBundle _mainAB = null;          //the main AssetBunld
    private AssetBundleManifest _manifest = null;    //the mainfest form the mainAB
    private Dictionary<string, AssetBundle> _abDic = new Dictionary<string, AssetBundle>();  //to record which ab has be loaded

    /// <summary>
    /// The assetBundle read place
    /// </summary>
    private string _PathUrl
    {
        get
        {
            return Application.streamingAssetsPath ;
        }
    }

    private string _MainName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// Load the MainAB
    /// </summary>
    private void LoadMain()
    {
        if(_mainAB == null)
        {
            _mainAB = AssetBundle.LoadFromFile($"{_PathUrl}/{_MainName}");
            if(_mainAB != null)
            {
                _manifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            else
            {
                Debug.LogError("The Main AB is not be find");
            }
        }
    }

    /// <summary>
    /// Load Res form the ab
    /// </summary>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callback, bool isSync = false) where T : Object
    {
        StartCoroutine(RealLoadRes<T>(abName, resName, callback, isSync));
    }

    private IEnumerator RealLoadRes<T>(string abName, string resName, UnityAction<T> callback, bool isSync = false) where T : Object
    {
        LoadMain();

        //Load Dependenies
        string[] dependeniesNameArray = _manifest.GetAllDependencies(abName);
        for(int i = 0; i < dependeniesNameArray.Length; i++)
        {
            if (isSync)
            {
                if (!_abDic.ContainsKey(dependeniesNameArray[i]))
                {
                    _abDic.Add(dependeniesNameArray[i], AssetBundle.LoadFromFile($"{_PathUrl}/{dependeniesNameArray[i]}"));
                }
            }
            else
            {
                if (!_abDic.ContainsKey(dependeniesNameArray[i]))
                {
                    print("DependName is " + dependeniesNameArray[i]);
                    //Load this ab with async
                    _abDic.Add(dependeniesNameArray[i], null);
                    var abcr = AssetBundle.LoadFromFileAsync($"{_PathUrl}/{dependeniesNameArray[i]}");

                    yield return abcr;

                    _abDic[dependeniesNameArray[i]] = abcr.assetBundle;
                }
                else
                {
                    //means that this ab is been loading now, just wait
                    while(ReferenceEquals(_abDic[dependeniesNameArray[i]], null))
                    {
                        yield return null;
                    }
                }
            }
        }

        //Load Target
        if (isSync)
        {
            if (!_abDic.ContainsKey(abName))
            {
                _abDic.Add(abName, AssetBundle.LoadFromFile($"{_PathUrl}/{abName}"));
            }
        }
        else
        {
            if (!_abDic.ContainsKey(abName))
            {
                _abDic.Add(abName, null);
                var abcr = AssetBundle.LoadFromFileAsync($"{_PathUrl}/{abName}");

                yield return abcr;

                _abDic[abName] = abcr.assetBundle;
            }
            else
            {
                while(ReferenceEquals(_abDic[abName], null))
                {
                    yield return null;
                }
            }
        }


        //Load Res
        if (isSync)
        {
            T res = _abDic[abName].LoadAsset<T>(resName);
#if UNITY_EDITOR
            if(res is GameObject)
            {
                AssetBundleEditorUtil.FixShadersForEditor(res as GameObject);
            }
#endif
            callback?.Invoke(res);
        }
        else
        {
            var abr = _abDic[abName].LoadAssetAsync<T>(resName);

            yield return abr;
            T res = abr.asset as T;
#if UNITY_EDITOR
            if(res is GameObject)
            {
                AssetBundleEditorUtil.FixShadersForEditor(res as GameObject);
            }
#endif
            callback?.Invoke(res as T);
        }
    }

    /// <summary>
    /// Unload Certain AssetBundle
    /// </summary>
    /// <param name="abName"></param>
    public void UnLoadAB(string abName, bool unLoadAllLoadedObj, UnityAction<bool> callBackResult)
    {
        if (_abDic.ContainsKey(abName))
        {
            if(_abDic[abName] == null)
            {
                //the ab is still loading
                callBackResult?.Invoke(false);
                return;
            }
            else
            {
                _abDic[abName].Unload(unLoadAllLoadedObj);
                _abDic.Remove(abName);
                callBackResult?.Invoke(false);
                return;
            }
        }
    }

    /// <summary>
    /// Unload all the loaded abs
    /// </summary>
    /// <param name="unLoadAllLoadedObj"></param>
    public void UnLoadAll(bool unLoadAllLoadedObj)
    {
        //Stop All the coruntine
        StopAllCoroutines();
        //Unload All the abs
        AssetBundle.UnloadAllAssetBundles(unLoadAllLoadedObj);
        //Clear the abDic
        _abDic.Clear();

        //unload the main
        _mainAB = null;
        _manifest = null;
    }
}
