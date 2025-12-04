using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class UIManager : Singleton<UIManager>
{
    private UIManager()
    {
        Awake();
    }

    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();   

    private EventSystem uiEventSystem;
    private Camera uiCamera;
    private Canvas uiCanvas;

    private Transform bottom;
    private Transform middle;
    private Transform top;
    private Transform system;

    private string PANEL_PATH = "UI/Panel/";

    public Canvas _Canvas => uiCanvas;
    public Camera _UICamera => uiCamera;

    public enum E_Layer
    { 
        Bottom,Middle,Top,System
    }


    private void Awake()
    {
        uiCamera = GameObject.Instantiate(Resources.Load<Camera>("UI/UICamera")).GetComponent<Camera>();
        GameObject.DontDestroyOnLoad(uiCamera.gameObject);

#if true
        AddUICameraToStack();

#endif
        uiCanvas = GameObject.Instantiate(Resources.Load<Canvas>("UI/Canvas")).GetComponent<Canvas>();
        uiCanvas.worldCamera = uiCamera;
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

        bottom = uiCanvas.transform.Find("Bottom");
        middle = uiCanvas.transform.Find("Middle");
        top = uiCanvas.transform.Find("Top");
        system = uiCanvas.transform.Find("System");

        uiEventSystem = GameObject.Instantiate(Resources.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
    }


    private Transform GetLayer(E_Layer layer)
    {
        switch (layer) {
            case E_Layer.Bottom:
                return bottom;
            case E_Layer.Middle:
                return middle;
            case E_Layer.Top:
                return top;
            case E_Layer.System:
                return system;
        }
        return null;
    }

    public T ShowPanel<T>(E_Layer layer = E_Layer.Middle) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName)) {
            return panelDic[panelName] as T;
        }
        else { 
            GameObject newPanelObj = GameObject.Instantiate(Resources.Load<GameObject>(PANEL_PATH +  panelName), GetLayer(layer));
            T newPanel = newPanelObj.GetComponent<T>();
            newPanel.ShowMe();
            panelDic.Add(panelName, newPanel);
        }

        return panelDic[panelName] as T;
    }


    public void HidePanel<T>() where T : BasePanel
    {
        string panelName = typeof (T).Name;

        if (panelDic.ContainsKey(panelName)) {
            T panel = panelDic[panelName] as T;

            panel.HideMe(() => {
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            });
        }
    }

    public void HidePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName)) {
            BasePanel panel = panelDic[panelName];
            panel.HideMe(() => {
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            });
        }
    }

    public T GetPanel<T>()  where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName)) 
            return panelDic[panelName] as T;
        

        return null;
    }

    public void Clear()
    {
        foreach (var panel in panelDic.Values) {
            GameObject.Destroy(panel.gameObject);
        }

        panelDic.Clear();
    }

    public void AddUICameraToStack()
    {
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(uiCamera);
    }
}
