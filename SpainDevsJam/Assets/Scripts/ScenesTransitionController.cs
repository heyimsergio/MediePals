using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesTransitionController : MonoBehaviour
{

    public void CloseScene(string newSceneName)
    {
        GetComponent<Animator>().SetTrigger("closeBox");
        StartCoroutine(LoadNewScene(newSceneName));
    }
     IEnumerator LoadNewScene(string newSceneName)
    {
        yield return new WaitForSecondsRealtime(1.05f);
        SceneManager.LoadSceneAsync(newSceneName);
        Time.timeScale = 1;
    }
}
