using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BridgeMove : MonoBehaviour
{
    public List<string> triggerWords;
    private Animator animator;
    private bool activated = false;
    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (activated)
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

        if (triggerWords.Contains(word))
        {
            StartCoroutine(StartAnimation());

        }
        IEnumerator StartAnimation()
        {
            animator.SetTrigger("isMoveBridge");
            activated = true;
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadSceneAsync("StageClear", LoadSceneMode.Additive);
            yield return null;
        }
    }
}
