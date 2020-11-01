using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetweenScenePass : MonoBehaviour
{

    public static BetweenScenePass instance;

    [Header("GamesPlayed")]
    [SerializeField] Dictionary<Games, bool> gamesPlayed;
    [SerializeField] Dictionary<Games, string> gamesScenes;

    public enum Games { Swords, Sacks, Chairs, Jousting, Baloons };
    [SerializeField] bool gameRestarted = true;

    [Header("Skins")]
    [SerializeField] int playerCharacterId;
    [SerializeField] List<int> npcCharacterId;

    [Space(10)]
    [SerializeField] Dictionary<Games, Dictionary<int, int>> characterScore;


    public bool GameRestarted { get => gameRestarted; set => gameRestarted = value; }
    public int PlayerCharacterId { get => playerCharacterId; set => playerCharacterId = value; }
    public Dictionary<Games, bool> GamesPlayed { get => gamesPlayed; set => gamesPlayed = value; }
    public Dictionary<Games, string> GamesScenes { get => gamesScenes; set => gamesScenes = value; }
    public Dictionary<Games, Dictionary<int, int>> CharacterScore { get => characterScore; set => characterScore = value; }
    public List<int> NpcCharacterId { get => npcCharacterId; set => npcCharacterId = value; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameRestarted = true;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (GameRestarted)
        {
            Debug.Log("Restart Game");
            RestartData();
        }

    }

    public void RestartData()
    {
        GamesPlayed = new Dictionary<Games, bool>();
        GamesScenes = new Dictionary<Games, string>();

        GamesPlayed[Games.Swords] = false;
        GamesPlayed[Games.Sacks] = false;
        GamesPlayed[Games.Chairs] = false;
        GamesPlayed[Games.Jousting] = false;
        GamesPlayed[Games.Baloons] = false;

        GamesScenes[Games.Swords] = "Scenes/Espadas";
        GamesScenes[Games.Sacks] = "Scenes/CarreraSacos";
        GamesScenes[Games.Chairs] = "Scenes/Sillas";
        GamesScenes[Games.Jousting] = "Scenes/Justa";
        GamesScenes[Games.Baloons] = "Scenes/Globos";

        PlayerCharacterId = -1;
        NpcCharacterId = new List<int>();

        CharacterScore = new Dictionary<Games, Dictionary<int, int>>();

        CharacterScore[Games.Swords] = new Dictionary<int, int>();
        for(int i = 0; i < 6; i++)
        {
            CharacterScore[Games.Swords][i] = 0;
        }

        CharacterScore[Games.Sacks] = new Dictionary<int, int>();
        for (int i = 0; i < 6; i++)
        {
            CharacterScore[Games.Sacks][i] = 0;
        }
        CharacterScore[Games.Chairs] = new Dictionary<int, int>();
        for (int i = 0; i < 6; i++)
        {
            CharacterScore[Games.Chairs][i] = 0;
        }
        CharacterScore[Games.Jousting] = new Dictionary<int, int>();
        for (int i = 0; i < 6; i++)
        {
            CharacterScore[Games.Jousting][i] = 0;
        }
        CharacterScore[Games.Baloons] = new Dictionary<int, int>();
        for (int i = 0; i < 6; i++)
        {
            CharacterScore[Games.Baloons][i] = 0;
        }


    }

    public IEnumerator DistributeScore(Games playerGame, int winner, ScenesTransitionController scenesTransition)
    {

        Debug.Log("SE ASIGNA 1 PUNTO AL PERSONAJE " + winner);

        yield return new WaitForSeconds(1f);
        CharacterScore[playerGame][winner] += 1;
        GamesPlayed[playerGame] = true;
        if (scenesTransition != null)
        {
            GameRestarted = false;

            if (GamesPlayed[Games.Swords] &&
                GamesPlayed[Games.Sacks] &&
                GamesPlayed[Games.Chairs] &&
                GamesPlayed[Games.Jousting] &&
                GamesPlayed[Games.Baloons])
            {
                AudioManager.instance.StopAll();
                scenesTransition.CloseScene("Scenes/EndGame");
            }
            else
            {
                AudioManager.instance.StopAll();
                scenesTransition.CloseScene("Scenes/Menu");
            }

        }
    }

    public IEnumerator DistributeScore(Games playerGame, int winner, ScenesTransitionController scenesTransition, float delay)
    {

        yield return new WaitForSeconds(delay);
        StartCoroutine(DistributeScore(playerGame, winner, scenesTransition));
    }
}
