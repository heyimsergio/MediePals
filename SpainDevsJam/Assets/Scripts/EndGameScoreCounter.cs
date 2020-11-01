using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndGameScoreCounter : MonoBehaviour
{
    SortedDictionary<int, int> scoreByChar;

    [SerializeField] GameObject winnerChar;
    [SerializeField] List<GameObject> secondWinnerChar;
    [SerializeField] List<GameObject> looserChars;

    BetweenScenePass data;


    void Awake()
    {
        scoreByChar = new SortedDictionary<int, int>();
        scoreByChar[0] = 0;
        scoreByChar[1] = 0;
        scoreByChar[2] = 0;
        scoreByChar[3] = 0;
        scoreByChar[4] = 0;
        scoreByChar[5] = 0;

        StartCoroutine(CountScore());
    }

    IEnumerator CountScore()
    {
        data = BetweenScenePass.instance;

        yield return new WaitUntil(() => data != null);

        DistributeScoreByCharacter();
    }

    void DistributeScoreByCharacter()
    {
        Debug.Log("cha");

        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Swords].Count; i++)
        {
            scoreByChar[i] += data.CharacterScore[BetweenScenePass.Games.Swords][i];
        }
        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Sacks].Count; i++)
        {
            scoreByChar[i] += data.CharacterScore[BetweenScenePass.Games.Sacks][i];
        }
        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Chairs].Count; i++)
        {
            scoreByChar[i] += data.CharacterScore[BetweenScenePass.Games.Chairs][i];
        }
        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Jousting].Count; i++)
        {
            scoreByChar[i] += data.CharacterScore[BetweenScenePass.Games.Jousting][i];
        }
        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Baloons].Count; i++)
        {
            scoreByChar[i] += data.CharacterScore[BetweenScenePass.Games.Baloons][i];
        }
       

         for (int i = 0; i < 6; i++)
         {
             Debug.Log("char: " + i + " - score: " + scoreByChar[i]);
         }

        int winner = GetWinner(-1);

        scoreByChar.Remove(winner);
        int winner2 = GetWinner(winner);

        scoreByChar.Remove(winner2);
        int winner3 = GetWinner(winner2);

        scoreByChar.Remove(winner3);


        winnerChar.GetComponent<CharacterIdController>().SetNpcCustom(winner);
        winnerChar.GetComponent<CharacterIdController>().DanceDance(winner + 1);

        secondWinnerChar[0].GetComponent<CharacterIdController>().SetNpcCustom(winner2);
        secondWinnerChar[0].GetComponent<CharacterIdController>().DanceDance(winner2 + 1);

        secondWinnerChar[1].GetComponent<CharacterIdController>().SetNpcCustom(winner3);
        secondWinnerChar[1].GetComponent<CharacterIdController>().DanceDance(winner3 + 1);

        int looserIndex = 0;
        foreach (KeyValuePair<int, int> entry in scoreByChar)
        {
            looserChars[looserIndex].GetComponent<CharacterIdController>().SetNpcCustom(entry.Key);
            looserChars[looserIndex].GetComponent<CharacterIdController>().DanceDance(entry.Key + 1);
            looserIndex++;
        }

    }

    int GetWinner(int lastWinner)
    {
        int maxScore = 0;
        int winner = 0;

        foreach (KeyValuePair<int, int> entry in scoreByChar)
        {
            if (entry.Value > maxScore && entry.Key != lastWinner)
            {
                maxScore = entry.Value;
                winner = entry.Key;
            }
        }

        if (scoreByChar.ContainsKey(data.PlayerCharacterId) && scoreByChar[data.PlayerCharacterId] == maxScore)
        {
            return data.PlayerCharacterId;
        }

        return winner;
    }

}
