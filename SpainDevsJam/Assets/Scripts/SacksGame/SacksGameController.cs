using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SacksGameController : MonoBehaviour
{
    [SerializeField] ScenesTransitionController scenesTransition;

    [SerializeField] List<SaltosSacosController> players;

    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject countdownPanel;
    [SerializeField] GameObject gameOverPanel;

    private GameObject winner;

    [Header("Camera Settings")]
    [SerializeField] CinemachineVirtualCamera cameraGame;
    [SerializeField] CinemachineVirtualCamera cameraFinish;
    [SerializeField] float cinematicVelocity;

    bool scoreDistributed;

    private void Start()
    {
        StartCoroutine(AudioManager.instance.FadeIn("transicion", 1));
    }

    public void StartGame()
    {
        StartCoroutine(BeforeStartGame());
    }


    private IEnumerator BeforeStartGame()
    {
        //Fin Cinematica
        StartCoroutine(AudioManager.instance.FadeOut("transicion", 1));
        StartCoroutine(AudioManager.instance.FadeIn("06_sacos", 1));
        //AudioManager.instance.FadeIn("transicion", 1);

        yield return new WaitForSecondsRealtime(0.5f);
        countdownPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(4f);
        countdownPanel.SetActive(false);
        AudioManager.instance.PlayOneShot("SFX_silbato");
        foreach (SaltosSacosController player in players)
        {
            player.StartGame(this);
        }
        AudioManager.instance.FadeOut("transicion", 1);
        AudioManager.instance.FadeIn("06_sacos", 1);
    }

    public void GameOver(GameObject _winner)
    {
        StartCoroutine(AudioManager.instance.FadeOut("06_sacos", 1));
        winner = _winner;
        foreach (SaltosSacosController player in players)
        {
            if (winner.Equals(player.gameObject))
            {
                if (player.GetIsPlayable())
                {
                    AudioManager.instance.PlayOneShot("SFX_campanillas");
                }
            }
        }
        gameOverPanel.SetActive(true);
        cameraFinish.gameObject.SetActive(true);
        cameraGame.gameObject.SetActive(false);
        foreach (SaltosSacosController player in players)
        {
            player.GameOver();
        }

        if (!scoreDistributed)
        {
            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Sacks,
                winner.GetComponent<CharacterIdController>().CharacterId,
                scenesTransition,
                1.5f));

            scoreDistributed = true;
        }

    }

}
