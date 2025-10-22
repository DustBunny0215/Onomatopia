using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] private string loadSceneName;
    [SerializeField] private string unloadSceneName;
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(SetScene());
        }
    }

    IEnumerator SetScene()
    {
        
        yield return SceneManager.LoadSceneAsync(loadSceneName);
        yield return SceneManager.UnloadSceneAsync(unloadSceneName);

    }
}
