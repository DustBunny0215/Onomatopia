using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageGetCursor : MonoBehaviour
{
    private Vector3 originalScale;
    [SerializeField] private float scaleFactor = 1.05f;//大きくする倍率
    [SerializeField] private float duration = 0.2f; // アニメーション時間（秒）
    private Coroutine scaleCoroutine;

    [SerializeField] private string stageName;
    [SerializeField] private GameObject staLevTex;

    [SerializeField] private int stageNumber;
    [SerializeField] private GameObject lockIcon;
    public bool isLock=false;
    private void Start()
    {
        int unlocked = GameProgressManager.GetUnlockedStage();
        originalScale = transform.localScale;

        if (stageNumber <= unlocked)
        {
            isLock = false;
            if (lockIcon != null) lockIcon.SetActive(false);
        }
        else
        {
            isLock = true;
            if (lockIcon != null) lockIcon.SetActive(true);
        }
    }

    void OnMouseEnter()
    {
        if (isLock != true)
        {
            StartScale(originalScale * scaleFactor);
        }
        
        staLevTex.GetComponent<Text>().text = stageName;

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
