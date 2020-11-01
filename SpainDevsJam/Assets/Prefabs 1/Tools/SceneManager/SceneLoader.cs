using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour{

    Animator anim;
    [SerializeField, Range(0f, 3f)] float fadeDuration = 1f;

    public static SceneLoader instance;

    void Awake(){
        instance = this;
        anim = GetComponent<Animator>();
    }

    /// <summary> Loads the scene. </summary>
    public void LoadScene(int scene){
        StartCoroutine(LoadLevel(scene));
    }

    IEnumerator LoadLevel(int scene){
        anim.SetTrigger("Fade");
        yield return new WaitForSecondsRealtime(fadeDuration);
        SceneManager.LoadSceneAsync(scene);
    }

    /// <summary> Loads the scene with a certain delay. </summary>
    public void LoadScene(int scene, float delay){
        StartCoroutine(LoadLevel(scene, delay));
    }

    IEnumerator LoadLevel(int scene, float delay){
        yield return new WaitForSecondsRealtime(delay);
        anim.SetTrigger("Fade");
        yield return new WaitForSecondsRealtime(fadeDuration);
        SceneManager.LoadSceneAsync(scene);
    }
}
