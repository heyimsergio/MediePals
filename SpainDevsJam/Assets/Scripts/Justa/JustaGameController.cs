using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JustaGameController : MonoBehaviour
{
    [SerializeField] ScenesTransitionController scenesTransition;
    [SerializeField] int actualRound = 1;

    [SerializeField] JustaPlayerController player;
    [SerializeField] JustaPlayerController enemy;

    [SerializeField] bool paused;

    [SerializeField] GameObject countDownPanel;

    [SerializeField] JustaTargetComboObjects[] targetObjects;

    [SerializeField] Image player_HPBar;
    [SerializeField] Image enemy_HPBar;

    [SerializeField] JustaTrigger trigger;

    [SerializeField] GameObject gameHUD;

    bool scoreDistributed;

    // Start is called before the first frame update
    void Start()
    {
        //BeforeGame();
        StartCoroutine(AudioManager.instance.FadeIn("transicion", 1));
    }

    private void BeforeGame()
    {
        StartCoroutine(BeforeGameCoroutine());
    }

    private IEnumerator BeforeGameCoroutine()
    {

        //Cinematica inicial
        yield return new WaitForSecondsRealtime(2);
        //Tutorial
        StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(AudioManager.instance.FadeOut("transicion", 1));
        StartCoroutine(AudioManager.instance.FadeIn("02_caballos", 1));
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        ShowGameHUD();
        while (!GameFinished())
        {
            Debug.Log("Ronda: " + actualRound);
            Debug.Log("Vida enemy " + enemy.GetCurrentLives() + " vida player " + player.GetCurrentLives());
            SetNewCombo();
            countDownPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(4f);
            AudioManager.instance.PlayOneShot("SFX_silbato");
            countDownPanel.SetActive(false);
            trigger.paused = false;
            enemy.StartRunning();
            player.StartRunning();
            yield return new WaitForSecondsRealtime(9f);
            trigger.paused = true;
            CreateEnemyPuntuation();
            CheckRoundWinner();
            actualRound++;
        }
        yield return null;
    }

    private void CreateEnemyPuntuation()
    {
        float points = Random.Range(1f, 5f);
        enemy.SetRoundPoints(points);
    }

    private bool GameFinished()
    {
        if (player.GetCurrentLives() <= 0)
        {
            ReturnToMenu(enemy.GetComponent<CharacterIdController>().characterId, player.GetComponent<CharacterIdController>().characterId);
            return true;
        }
        if (enemy.GetCurrentLives() <= 0)
        {
            ReturnToMenu(player.GetComponent<CharacterIdController>().characterId, enemy.GetComponent<CharacterIdController>().characterId);
            return true;
        }
        return false;
    }

    private void CheckRoundWinner()
    {
        float enemyPoints = enemy.GetRoundPoints();
        float playerPoints = player.GetRoundPoints();
        if (enemyPoints <= playerPoints)
        {
            Debug.Log("Has ganado la ronda: enemigo: " + enemyPoints + " tuyos:" + playerPoints);
            AudioManager.instance.PlayOneShot("SFX_campanillas");
            enemy_HPBar.fillAmount -= 0.2f;
            LooseLife(enemy);
        }
        else
        {
            Debug.Log("Has ganado la ronda: enemigo: " + enemyPoints + " tuyos:" + playerPoints);
            player_HPBar.fillAmount -= 0.2f;
            LooseLife(player);
        }
        enemy.SetRoundPoints(0);
        player.SetRoundPoints(0);
    }

    private void LooseLife(JustaPlayerController character)
    {
        character.LooseLife();
    }

    private void SetNewCombo()
    {
        foreach (JustaTargetComboObjects target in targetObjects)
        {
            target.SetRandomKey();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowGameHUD()
    {
        gameHUD.SetActive(true);
    }

    void ReturnToMenu(int winner, int looser) {
        StartCoroutine(AudioManager.instance.FadeOut("02_caballos", 1));
        if (!scoreDistributed)
        {
            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Jousting,
                winner,
                null));

            int newWinner1 = winner;
            int i = 0;
            while (i <100)
            {
                newWinner1 = Random.Range(0, 6);
                i++;
                if (newWinner1 != winner && newWinner1 != looser)
                {
                    break;
                }
            }

            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Jousting,
                newWinner1,
                null));

            int newWinner2;
            newWinner2 = winner;
            i = 0;
            while (i <100)
            {
                newWinner2 = Random.Range(0, 6);
                i++;
                if (newWinner2 != winner && newWinner2 != looser && newWinner2 != newWinner1)
                {
                    break;
                }
            }

            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Jousting,
                newWinner2,
                scenesTransition));
            scoreDistributed = true;
        }
    }
}
