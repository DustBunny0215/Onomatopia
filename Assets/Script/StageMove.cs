using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StageMove : MonoBehaviour
{
    [SerializeField] private string stageName;
    private bool isMouseHere=false;

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
        if (Input.GetMouseButtonDown(0)&&isMouseHere==true)
        {
            StartCoroutine(SetStage());
        }
    }
    IEnumerator SetStage()
    {
        yield return SceneManager.LoadSceneAsync(stageName, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(0,LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync("ChoiceStage");
        Debug.Log("Ко");
    }
}
