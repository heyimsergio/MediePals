using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightRingController : MonoBehaviour
{
    [SerializeField] Animator textAnim;
    [SerializeField] ScenesTransitionController scenesTransition;
    [SerializeField] GameObject ringParent;
    [SerializeField] Animator anim; 
    [SerializeField] BetweenScenePass.Games game;

    [SerializeField] bool isOnRange;

    BetweenScenePass gameData;

    private void Start()
    {
        if(gameData == null)
            gameData = BetweenScenePass.instance;

        ringParent.SetActive(!gameData.GamesPlayed[game]);
    }

    void HideRing()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isOnRange && Input.GetMouseButtonDown(0) && Time.timeScale == 1)
        {
            anim.SetTrigger("start");

            StartCoroutine(AudioManager.instance.FadeOut("01_jardin", 1));
            StartCoroutine(AudioManager.instance.FadeOut("SFX_aura", .5f));
            scenesTransition.CloseScene(gameData.GamesScenes[game]);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AudioManager.instance.FadeIn("SFX_aura", .5f));
            isOnRange = true;
            anim.SetTrigger("up");

            textAnim.gameObject.SetActive(true);
            //textAnim.SetTrigger("textUp");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AudioManager.instance.FadeOut("SFX_aura", .5f));
            StartCoroutine(AudioManager.instance.FadeIn("SFX_vocesBackground", .5f));
            isOnRange = false;
            anim.SetTrigger("down");

            textAnim.SetTrigger("textDown");
            StartCoroutine(HideText());
        }
    }

    IEnumerator HideText()
    {
        yield return new WaitForSecondsRealtime(0.21f);
        textAnim.gameObject.SetActive(false);
    }


}
