using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Video;

public class EditorResMgr : Singleton<EditorResMgr>
{
    private EditorResMgr(){}

     //the path where to save the res will be package into AB pack 
    private readonly static string _rootPath = "Assets/Editor/ArtRes/";

    //1.Load Single Res
    public T LoadEditorRes<T>(string path) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        string suffixName = "";
        if(typeof(T) == typeof(GameObject))
        {
            suffixName = ".prefab";
        }
        else if(typeof(T) == typeof(Material))
        {
            suffixName = ".mat";
        }
        else if(typeof(T) == typeof(Texture) || typeof(T) == typeof(Sprite))
        {
            suffixName = ".png";
        }
        else if(typeof(T) == typeof(AudioClip))
        {
            suffixName =  ".mp3";
        }
        else if(typeof(T) == typeof(VideoClip))
        {
            suffixName = ".mp4";
        }
        else if(typeof(T) == typeof(SpriteAtlas))
        {
            suffixName = ".spriteatlasv2";
        }
        else if(typeof(T) == typeof(ScriptableObject))
        {
            suffixName = ".asset";
        }

        return AssetDatabase.LoadAssetAtPath<T>(_rootPath + path + suffixName);
#else
        return null;
#endif
    }

    //2.Load Atlas Res
    public Sprite LoadSprite(string path, string spriteName)
    {
#if UNITY_EDITOR
        //get all son res in this atlas
        UnityEngine.Object[] spriteArray = AssetDatabase.LoadAllAssetRepresentationsAtPath(_rootPath + path + ".spriteatlasv2");
        foreach(var sprite in spriteArray)
        {
            if(sprite.name == spriteName)
            {
                return sprite as Sprite;
            }
        }
        return null;
#else
        return null;
#endif        
        
    }

    //3.Load Atlas's all Sprites and return 
    public Dictionary<string, Sprite> LoadSpriteDic(string path)
    {
#if UNITY_EDITOR
        UnityEngine.Object[] spriteArray = AssetDatabase.LoadAllAssetRepresentationsAtPath(_rootPath + path + ".spriteatlasv2");
        Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
        foreach(var sprite in spriteArray)
        {
            spriteDic.Add(sprite.name, sprite as Sprite);
        }
        return spriteDic;
#else
        return null;
#endif        
    }
}
