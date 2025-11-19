using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IwaWithBridge : MonoBehaviour
{
    public List<string> iwaTriggerWords;
    private Animator animator;
    public bool activated = false;
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

        if (iwaTriggerWords.Contains(word))
        {
            ClearData.clearedStage = 1;
            StartCoroutine(StartAnimation());

        }
        IEnumerator StartAnimation()
        {
            animator.SetBool("isMove2", true);
            activated = true;
            yield return null;
        }
    }
}
