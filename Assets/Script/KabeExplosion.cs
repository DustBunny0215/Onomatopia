using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KabeExplosion : MonoBehaviour
{

    public List<string> triggerWords;
    private Animator animator;
    private bool activated = false;
    private GameObject iwakabe;
    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        iwakabe = transform.GetChild(0).gameObject;
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
            ClearData.clearedStage = 3;
            StartCoroutine(StartAnimation());

        }
        IEnumerator StartAnimation()
        {
            animator.SetTrigger("isBomb");
            activated = true;
            yield return new WaitForSecondsRealtime(1f);
            iwakabe.SetActive(false);
            SceneManager.LoadSceneAsync("StageClear", LoadSceneMode.Additive);
            yield return null;
        }
    }
}
