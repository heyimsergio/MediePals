using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameController : MonoBehaviour
{

    [SerializeField] CMCameraRail rail;

    [SerializeField] CanvasGroup credits;

    bool creditsAlpha = false;

    // Start is called before the first frame update
    void Start()
    {
        rail.StartRail();
        StartCoroutine(AudioManager.instance.FadeIn("final_minijuego", 1));
        StartCoroutine(PlayKidsSound(3, 6));
    }

    // Update is called once per frame
    void Update()
    {
        if (creditsAlpha)
        {
            credits.alpha += Time.deltaTime;
            
            if(credits.alpha >= 1)
            {
                creditsAlpha = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void ShowCredits()
    {
        creditsAlpha = true;
    }

    private IEnumerator PlayKidsSound(float maxDelay, float minDelay)
    {
        float i = 0;
        float delay = 0;
        while (i < 100000)
        {
            delay = Random.Range(minDelay, maxDelay);
            i += Time.deltaTime;
            AudioManager.instance.PlayOneShot("SFX_cheering");
            yield return new WaitForSecondsRealtime(delay);
        }
    }
}
