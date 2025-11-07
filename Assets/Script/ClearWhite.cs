using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearWhite : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private float speed;
    [SerializeField] private float cla;
    [SerializeField] private string stageName;
    [SerializeField] private StageData stageData;

    [SerializeField] private int currentStage;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = ClearData.clearedStage;
        stageName = stageData.currentStageName;
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(Display());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Display()
    {
        GameProgressManager.UnlockStage(currentStage + 1);

        while (cla < 1f)
        {
            cla += speed;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, cla);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f);
        yield return SceneManager.LoadSceneAsync("ChoiceStage", LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync(stageName);
        yield return SceneManager.UnloadSceneAsync("VoiceRecogSystem");
        yield return SceneManager.UnloadSceneAsync("MassageUI");
        yield return SceneManager.UnloadSceneAsync("StageClear");
    }
}
