using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGetCursor : MonoBehaviour
{
    private Vector3 originalScale;
    [SerializeField] private float scaleFactor = 1.05f;//大きくする倍率
    [SerializeField] private float duration = 0.2f; // アニメーション時間（秒）
    private Coroutine scaleCoroutine;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        StartScale(originalScale * scaleFactor);

    }

    void OnMouseExit()
    {
        StartScale(originalScale);
    }

    void StartScale(Vector3 targetScale)
    {
        // 前のアニメーションを止める
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleOverTime(targetScale));
    }

    System.Collections.IEnumerator ScaleOverTime(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            // スムーズステップ（Ease In-Out）
            t = t * t * (3f - 2f * t);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; 
    }
}
