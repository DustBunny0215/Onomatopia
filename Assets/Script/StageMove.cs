using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StageMove : MonoBehaviour
{
    [SerializeField] private string stageName;
    [SerializeField] private StageData stageData;
    private bool isMouseHere=false;

    [SerializeField] private StageGetCursor staGetCur;

    private void OnMouseEnter()
    {
        isMouseHere = true;
    }
    private void OnMouseExit()
    {
        isMouseHere = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&isMouseHere==true&&staGetCur.isLock==false)
        {
            StartCoroutine(SetStage());
        }
    }
    IEnumerator SetStage()
    {
        stageData.currentStageName = stageName;
        yield return SceneManager.LoadSceneAsync("VoiceRecogSystem", LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync("MassageUI", LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(stageName, LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync("ChoiceStage");
    }
}
