using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BridgeWithIwa : MonoBehaviour
{
    [SerializeField] private IwaWithBridge iwb;
    private Animator animator;
    public List<string> bridgeTriggerWords;

    private bool clearProcessStarted = false;   // ← これが超重要（前と同じガード）

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // すでにクリア処理が始まっていれば絶対に何もしない（無限Additive防止）
        if (clearProcessStarted)
        {
            return;
        }

        if (VoiceManager.Instance == null)
        {
            return;
        }

        string word = VoiceManager.Instance?.recognizedWord;

        if (string.IsNullOrEmpty(word))
        {
            return;
        }

        if (bridgeTriggerWords.Contains(word))
        {
            // 岩が動作可能状態でなければ無視
            if (!iwb.activated)
            {
                Debug.Log("miss");
                return;
            }

            // ★ ここでクリア処理開始を “1度だけ” にするガード
            clearProcessStarted = true;

            ClearData.clearedStage = 4;
            StartCoroutine(StartAnimation());
        }
    }

    private IEnumerator StartAnimation()
    {
        animator.SetTrigger("isMoveBridge");
        yield return new WaitForSecondsRealtime(1f);

        // Additiveで読み込み（ここは一回だけ確実に実行される）
        SceneManager.LoadScene("StageClear", LoadSceneMode.Additive);
    }
}

