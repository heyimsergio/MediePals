using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using System.Threading;

public class ChairsGameController : MonoBehaviour
{
    [SerializeField] ScenesTransitionController scenesTransition;
    [SerializeField] CinemachineVirtualCamera cameraCutscene;
    [SerializeField] CinemachineVirtualCamera cameraGame;
    [SerializeField] CinemachineVirtualCamera cameraEndOfRound;

    [SerializeField] float initialCinematicVelocity;

    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject countdownPanel;
    [SerializeField] GameObject gameOverPanel;


    [SerializeField] float timeBetweenRoundsAfterCam;
    [SerializeField] float velocityCamBetweenRounds;

    [SerializeField] NavMeshSurface surface;

    [SerializeField] GameObject[] obstacles1;
    [SerializeField] GameObject[] obstacles2;
    [SerializeField] GameObject[] obstacles3;
    [SerializeField] GameObject[] obstacles4;


    [SerializeField] ChairsChairController[] chairs;
    /*[HideInInspector]*/
    public List<ChairsChairController> chairsPlaying;
    /*[HideInInspector]*/
    public List<ChairsChairController> freeChairs;

    [SerializeField] ChairsEnemyController[] enemies;
    /*[HideInInspector]*/
    public List<ChairsEnemyController> enemiesPlaying;

    [SerializeField] ChairsPlayerController player;

    [SerializeField] int numberOfRounds;
    [SerializeField] private int actualRound = 1;

    private bool prueba = false;

    bool scoreDistributed;


    private void Awake()
    {
        chairsPlaying = new List<ChairsChairController>(chairs);
        enemiesPlaying = new List<ChairsEnemyController>(enemies);
        StartCoroutine(AudioManager.instance.FadeIn("transicion", 1));
        PrepareMap();
    }

    private void Start()
    {
    }

    public void StartGame()
    {
        StartCoroutine(BeforeStartGame());
    }

    private IEnumerator BeforeStartGame()
    {
        //Fin Cinematica
        yield return new WaitForSecondsRealtime(0.5f);
        countdownPanel.SetActive(true);
        StartCoroutine(AudioManager.instance.FadeOut("transicion", 1));
        StartCoroutine(AudioManager.instance.FadeIn("03_sillas", 1));
        yield return new WaitForSecondsRealtime(4f);
        //tutorialPanel.SetActive(false);
        countdownPanel.SetActive(false);
        AudioManager.instance.PlayOneShot("SFX_silbato");
        SetChairs();
        StartEnemies();

        InitRound();
        
    }

    private void StartEnemies()
    {
        foreach (ChairsEnemyController enemy in enemies)
        {
            enemy.InitGame();
        }
    }

    private void InitRound()
    {
        PausePlayers(false);
    }

    private void ResetMap()
    {
        foreach (GameObject obstacle in obstacles1)
        {
            if (!obstacle.activeSelf)
            {
                obstacle.SetActive(true);
                break;
            }
        }
        foreach (GameObject obstacle in obstacles2)
        {
            if (!obstacle.activeSelf)
            {
                obstacle.SetActive(true);
                break;
            }
        }
        foreach (GameObject obstacle in obstacles3)
        {
            if (!obstacle.activeSelf)
            {
                obstacle.SetActive(true);
                break;
            }
        }
        foreach (GameObject obstacle in obstacles4)
        {
            if (!obstacle.activeSelf)
            {
                obstacle.SetActive(true);
                break;
            }
        }
    }

    private void PrepareMap()
    {
        ResetMap();
        int obstacleToEliminate = Random.Range(0, obstacles1.Length);
        obstacles1[obstacleToEliminate].SetActive(false);
        obstacleToEliminate = Random.Range(0, obstacles2.Length);
        obstacles2[obstacleToEliminate].SetActive(false);
        obstacleToEliminate = Random.Range(0, obstacles3.Length);
        obstacles3[obstacleToEliminate].SetActive(false);
        obstacleToEliminate = Random.Range(0, obstacles4.Length);
        obstacles4[obstacleToEliminate].SetActive(false);
        //surface.BuildNavMesh();
    }

    private void SetChairs()
    {
        freeChairs = new List<ChairsChairController>(chairsPlaying);
    }

    private void SetEnemies()
    {
        enemiesPlaying = new List<ChairsEnemyController>(enemies);
    }

    public void OccupyChair(ChairsChairController chair)
    {
        freeChairs.Remove(chair);
        if (freeChairs.Count == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Fin de ronda");
        if (!player.sat)
        {
            player.sat = false;
            Debug.Log("Termina porque player no sentado, ronda: " + actualRound);
            FinishGame();
            return;
        }
        if (actualRound < numberOfRounds)
        {
            player.sat = false;
            actualRound++;
            StartCoroutine(WaitEndOfRound());
        }
        else
        {
            Debug.Log("Termina por rondas");
            FinishGame();
        }
    }

    private IEnumerator WaitEndOfRound()
    {
        //AudioManager.instance.Play("transicion");
        LooseEnemies(); //Para animacion
        PausePlayers(true);
        cameraGame.gameObject.SetActive(false);
        cameraEndOfRound.gameObject.SetActive(true);
        Vector3 initialPosOfCamera = cameraEndOfRound.transform.position;
        yield return new WaitForSecondsRealtime(timeBetweenRoundsAfterCam);
        PrepareMap();
        //yield return new WaitForSecondsRealtime(timeBetweenRoundsAfterCam/2);
        float i = 0;
        while (i < 1.5)
        {
            i += Time.deltaTime;
            cameraEndOfRound.transform.Translate(Vector3.back * Time.deltaTime * velocityCamBetweenRounds, Space.World);
            yield return null;
        }
        //yield return new WaitForSecondsRealtime(timeBetweenRoundsAfterCam/2);
        ResetPlayersPosition();
        EliminatePlayersAndChairs();
        i = 0;
        while (i < 1)
        {
            i += Time.deltaTime;
            cameraEndOfRound.transform.Translate(Vector3.back * Time.deltaTime * velocityCamBetweenRounds, Space.World);
            yield return null;
        }
        cameraGame.gameObject.SetActive(true);
        cameraEndOfRound.gameObject.SetActive(false);
        countdownPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(4);
        cameraEndOfRound.transform.position = initialPosOfCamera;
        countdownPanel.SetActive(false);
        PausePlayers(false);
        player.sat = false;
        foreach (ChairsEnemyController enemy in enemiesPlaying)
        {
            enemy.SetNewTargetChair();
        }
    }

    private void PausePlayers(bool mode)
    {

        player.paused = mode;
        foreach (ChairsEnemyController enemy in enemiesPlaying)
        {
            enemy.paused = mode;
        }

    }

    private void LooseEnemies()
    {
        foreach (ChairsEnemyController enemy in enemiesPlaying)
        {
            if (!enemy.sat)
            {
                enemy.Loose();
            }
        }
    }

    private void ResetPlayersPosition()
    {
        ResetPlayer();
        foreach (ChairsEnemyController enemy in enemiesPlaying)
        {
            //ResetEnemy(enemy);
            enemy.ResetEnemy();
        }
    }

    private void ResetPlayer()
    {
        //Debug.Log("Esta en:" + player.transform.position + " y quiere ir a " + player.startPosition);
        // Debug.Log("Reseteo del player");
        //player.sat = false;
        CharacterController charController = player.GetComponent<CharacterController>();
        charController.enabled = false;
        player.transform.position = player.startPosition;
        player.transform.rotation = new Quaternion(0, 0, 0, 0);
        charController.enabled = true;
        //player.gameObject.SetActive(true);
        player.stunned = false;
        player.ResetAnim();
        //player.sat = false;
        //Debug.Log("Ha llegado a: " + player.transform.position);
    }

    private void ResetEnemy(ChairsEnemyController enemy)
    {
        //enemy.gameObject.SetActive(false);
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        //agent.enabled = false;
        enemy.transform.position = enemy.startPosition;
        player.transform.rotation = new Quaternion(0, 0, 0, 0);
        //agent.enabled = true;
        //enemy.gameObject.SetActive(true);
        enemy.sat = false;
        enemy.stunned = false;
        //enemy.SetNewTargetChair();
    }

    private void EliminatePlayersAndChairs()
    {
        int indexToEliminate = -1;
        for (int i = 0; i < enemiesPlaying.Count; i++)
        {
            if (!enemiesPlaying[i].sat)
            {
                indexToEliminate = i;

            }
            else
            {
                enemiesPlaying[i].sat = false;
            }
        }
        ChairsEnemyController enemy = enemiesPlaying[indexToEliminate];
        enemy.gameObject.SetActive(false);
        enemiesPlaying.RemoveAt(indexToEliminate);
        player.sat = false;
        for (int i = 0; i < chairsPlaying.Count; i++)
        {
            chairsPlaying[i].occupied = false;
        }

        indexToEliminate = Random.Range(0, chairsPlaying.Count);
        ChairsChairController silla = chairsPlaying[indexToEliminate];
        silla.gameObject.SetActive(false);
        chairsPlaying.RemoveAt(indexToEliminate);
        SetChairs();
    }

    private void FinishGame()
    {
        Debug.Log("Fin de partida");
        //Ver en que ronda estaba cuando se perdio
        //Si no es el la ultima, calcular tu posicion e inventar las de los que quedan
        //Si es la ultima, ver si has ganado o perdido

        StartCoroutine(AudioManager.instance.FadeOut("03_sillas", 1));

        if (player.sat)
        {
            player.paused = true;
            ReturnToMenu(player.gameObject.GetComponent<CharacterIdController>().CharacterId);
        }
        else
        {
            player.Loose();
            if (BetweenScenePass.instance && BetweenScenePass.instance.NpcCharacterId.Count > 0)
                ReturnToMenu(BetweenScenePass.instance.NpcCharacterId[Random.Range(0, BetweenScenePass.instance.NpcCharacterId.Count - 1)]);
            else
                ReturnToMenu(3);
        }
    }

    private void Update()
    {
        if (prueba)
        {
            //Debug.Log(player.transform.position);
        }
    }

    void ReturnToMenu(int winner)
    {
        if (!scoreDistributed)
        {
            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Chairs,
                winner,
                scenesTransition));

            scoreDistributed = true;
        }
    }

}
