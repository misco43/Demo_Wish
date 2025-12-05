using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum CaptionState { Hidden, Appearing, Visible, Disappearing }

/// <summary>
/// player's text animtion
/// </summary>
public class Caption : MonoBehaviour
{
    [Header("Componet Ref")]
    public TMP_Text targetText;

    [Header("TypeWriter Effect")]
    public float typewriterInterval = 0.1f;
    public float fadeDuration = 0.3f;
    public float scaleDuration = 0.3f;

    [Header("Disappera Setting")]
    public float disappearInterval = 0.1f;
    public float fallDistance = 50f;
    public float fallDuration = 0.5f;
    public AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private TMP_TextInfo textInfo;
    private List<Tween> activeTweens = new List<Tween>();
    private Coroutine _corountine;

    // animState
    private CaptionState currentState = CaptionState.Hidden;
    public CaptionState State => currentState;

    /// <summary>
    /// when the caption start to play
    /// </summary>
    public event UnityAction OnCaptionStart;    
    /// <summary>
    /// when the caption totally disapper
    /// </summary>
    public event UnityAction OnCaptionEnd;

    // Init the text state
    private void Init(string content)
    {
        print("out side place init");
        //set text and active the text
        gameObject.SetActive(true);
        targetText.text = content;

        targetText.ForceMeshUpdate();
        textInfo = targetText.textInfo;

        for(int i = 0; i < textInfo.characterCount; i++) {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            //get the material and vertex index
            int materialInex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            //get the color of vertex
            Color32[] vertexColors = textInfo.meshInfo[materialInex].colors32;

            //change the vertex color
            for(int j = 0;j < 4; j++) {
                Color32 originalColor = vertexColors[vertexIndex + j];
                vertexColors[vertexIndex + j] = new Color32(originalColor.r, originalColor.g, originalColor.b, 0);
            }
        }

        //update text data  -- only update the color part
        targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

       
    }

    /// <summary>
    /// Start Play the show text Animation
    /// </summary>
    public void ShowText(string content)
    {
        if(_corountine != null)
            StopCoroutine(_corountine);

        //Before show animation, init its start data and state
        Init(content);
        _corountine =  StartCoroutine(TypewriterRoutine());
    }

    /// <summary>
    /// Start Play hide text Animation
    /// </summary>
    public void HideText()
    {
        if(_corountine != null)
            StopCoroutine(_corountine);

        _corountine = StartCoroutine(DisappearRoutine());
    }

    private readonly static WaitForSeconds _CAPTION_WAIT = new WaitForSeconds(CaptionMgr.CAPTION_STAY_TIME);

    // type writer effect Ienumerator
    private IEnumerator TypewriterRoutine()
    {
        currentState = CaptionState.Appearing;
        OnCaptionStart?.Invoke();

        // stop all current anim
        foreach (var tween in activeTweens)
            tween.Kill();
        activeTweens.Clear();

        //FIXME: this place is not need to ForceMeshUpdate, other wise it's material will be show out into default again;
        //targetText.ForceMeshUpdate();
        textInfo = targetText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++) {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            // get current char's index
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // create anim for this char
            //alpha anim
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            for (int j = 0; j < 4; j++) {
                int curIndex = vertexIndex + j;
                DOTween.To(() => vertexColors[curIndex].a,
                    (alpha) => {
                        Color32 color = vertexColors[curIndex];
                        color.a = (byte)(alpha * 255);
                        vertexColors[curIndex] = color;
                        targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                    },
                    1,
                    fadeDuration);
            }

            //scale anim
            Vector3[] vertexs = textInfo.meshInfo[materialIndex].vertices;
            Vector3[] originVertexs = new Vector3[4];
            for(int j = 0; j < 4; j++) {
                int curIndex = vertexIndex + j;
                originVertexs[j] = vertexs[curIndex];
            }
            Vector3 center = (originVertexs[0] + originVertexs[2]) * 0.5f;
            for(int j = 0; j < 4; j++) {
                int curIndex = vertexIndex + j;
                //TODO:Optimize
                Vector3 originPos = originVertexs[j];   //here is a closure��Maybe need to be optimisze in the future
                DOTween.To(() => 0f,
                    t => {
                        Vector3 offset = originPos - center;
                        vertexs[curIndex] = center + offset * t;
                        targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                    },
                    1f,
                    scaleDuration).SetEase(Ease.OutBack);
            }


            yield return new WaitForSeconds(typewriterInterval);
        }

        // wait for all chars appear
        yield return new WaitForSeconds(Mathf.Max(fadeDuration, scaleDuration));
        currentState = CaptionState.Visible;


        //TODO: wait some time and then play the disapper animation
        yield return _CAPTION_WAIT;
        HideText();
    }

    // disappear anim ienuerator
    private IEnumerator DisappearRoutine()
    {
        currentState = CaptionState.Disappearing;

        // stop breath effect
        foreach (var tween in activeTweens)
            tween.Kill();
        activeTweens.Clear();

        targetText.ForceMeshUpdate();
        textInfo = targetText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++) {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            AnimateCharacterDisappear(i, vertexIndex, materialIndex);

            yield return new WaitForSeconds(disappearInterval);
        }

        yield return new WaitForSeconds(fallDuration);
        currentState = CaptionState.Hidden;

        OnCaptionEnd?.Invoke();
        gameObject.SetActive(false);
    }

    // single char disappear anim
    private void AnimateCharacterDisappear(int charIndex, int vertexIndex, int materialIndex)
    {
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

        // record origin pos
        Vector3[] originalPositions = new Vector3[4];
        for (int j = 0; j < 4; j++) {
            originalPositions[j] = vertices[vertexIndex + j];
        }

        // fall anim sequne
        Sequence fallSequence = DOTween.Sequence();

        // fall down anim
        fallSequence.Append(DOTween.To(() => 0f, t => {
            for (int j = 0; j < 4; j++) {
                float fallOffset = fallDistance * t;
                float rotation = 360f * t; // rotate range

                vertices[vertexIndex + j] = originalPositions[j] + new Vector3(0, -fallOffset, 0);

                // simple rotate
                Vector3 center = (originalPositions[0] + originalPositions[2]) * 0.5f  + new Vector3(0, -fallOffset, 0);
                Vector3 offset = vertices[vertexIndex + j] - center;
                offset = Quaternion.Euler(0, 0, rotation) * offset;
                vertices[vertexIndex + j] = center + offset;
            }
            targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }, 1f, fallDuration).SetEase(fallCurve));



        // fade out at the same time
        fallSequence.Join(DOTween.To(() => 1f, alpha => {
            for (int j = 0; j < 4; j++) {
                Color32 color = vertexColors[vertexIndex + j];
                color.a = (byte)(alpha * 255);
                vertexColors[vertexIndex + j] = color;
            }
            targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }, 0f, fallDuration * 0.7f));

        activeTweens.Add(fallSequence);
    }

    // clear all the resources(stop all anim)
    private void OnDisable()
    {
        foreach (var tween in activeTweens)
            tween.Kill();

        if(OnCaptionEnd != null)
            OnCaptionEnd = null;
        if(OnCaptionStart != null)
            OnCaptionStart = null;
    }
}
