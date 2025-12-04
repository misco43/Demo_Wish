using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
    public float fadeTime = 0.5f;
    public  CanvasGroup canvasGroup;
    private UnityAction hideCallback;

    protected virtual void Start()
    {
        if(canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }


    public virtual void ShowMe()
    {
        StartCoroutine(ShowIEnumerator());
    }

    private IEnumerator ShowIEnumerator()
    {
        canvasGroup.alpha = 0;

        float timer = 0;

        while (timer <= fadeTime) {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    public virtual void HideMe(UnityAction callback) 
    {
        if (hideCallback != null)
            return;

        hideCallback += callback;

        StartCoroutine(HideIEnumerator());
    }

    private IEnumerator HideIEnumerator()
    {
        canvasGroup.alpha = 1;

        float timer = 0;

        while (timer <= fadeTime) {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 0;

        hideCallback?.Invoke();
    }

    protected virtual void  OnDestroy()
    {
        if(hideCallback != null) 
            hideCallback = null;
    }
}
