using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// player's text animtion
/// </summary>
public class PlayerTxtAnimation : MonoBehaviour
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
    private Coroutine currentAnimation;
    private List<Tween> activeTweens = new List<Tween>();

    // animState
    private enum AnimationState { Hidden, Appearing, Visible, Disappearing }
    private AnimationState currentState = AnimationState.Hidden;

    void Start()
    {
        if (targetText == null)
            targetText = GetComponent<TMP_Text>();

        // hide all text in the beginning
        InitializeText();
    }

    // Init the text state
    private void InitializeText()
    {
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

    // show text(type writer effect)
    public void ShowText()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(TypewriterRoutine());
    }

    // hide text£¨dropping effect£©
    public void HideText()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(DisappearRoutine());
    }

    // type writer effect Ienumerator
    private IEnumerator TypewriterRoutine()
    {
        currentState = AnimationState.Appearing;

        // stop all current anim
        foreach (var tween in activeTweens)
            tween.Kill();
        activeTweens.Clear();

        targetText.ForceMeshUpdate();
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
                Vector3 originPos = originVertexs[j];   //here is a closure¡£Maybe need to be optimisze in the future
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

        currentState = AnimationState.Visible;
    }

    // disappear anim ienuerator
    private IEnumerator DisappearRoutine()
    {
        currentState = AnimationState.Disappearing;

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
        currentState = AnimationState.Hidden;

        Destroy(gameObject);
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
    private void OnDestroy()
    {
        foreach (var tween in activeTweens)
            tween.Kill();
    }
}
