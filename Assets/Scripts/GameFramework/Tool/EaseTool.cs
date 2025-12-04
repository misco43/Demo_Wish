using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EaseTool
{
    public enum E_EaseType
    {
        None,
        UseCurve,
        ElasticEaseOut,
        BounceEaseOut,
        BackEaseOut
    }

    public static float Ease(E_EaseType type, float t, AnimationCurve curve = null)
    {
        switch (type) {
            case E_EaseType.ElasticEaseOut:
                return ElasticEaseOut(t);
            case E_EaseType.BounceEaseOut:
                return BounceEaseOut(t);
            case E_EaseType.BackEaseOut:
                return BackEaseOut(t);
            case E_EaseType.UseCurve:
                if(curve != null)
                    return curve.Evaluate(t);
                break;
        }

        return t;
    }

    // 数学缓动函数
    public static float ElasticEaseOut(float t)
    {
        if (t == 0 || t == 1) return t;
        return Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.1f) * (2 * Mathf.PI) / 0.4f) + 1;
    }

    public static float BounceEaseOut(float t)
    {
        if (t < (1f / 2.75f))
            return 7.5625f * t * t;
        else if (t < (2f / 2.75f))
            return 7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f;
        else if (t < (2.5f / 2.75f))
            return 7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f;
        else
            return 7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f;
    }

    public static float BackEaseOut(float t)
    {
        float s = 1.70158f;
        return 1 + (s + 1) * Mathf.Pow(t - 1, 3) + s * Mathf.Pow(t - 1, 2);
    }
}
