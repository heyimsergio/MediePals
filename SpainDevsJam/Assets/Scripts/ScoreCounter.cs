using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{

    [SerializeField] List<GameObject> scores;

    [SerializeField] List<int> scorePoints;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartScoreCount());
    }

    IEnumerator StartScoreCount()
    {
        BetweenScenePass data = BetweenScenePass.instance;

        yield return new WaitUntil(() => data != null);

        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Swords].Count; i++)
        {
            scorePoints[i] += data.CharacterScore[BetweenScenePass.Games.Swords][i];
        }
        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Sacks].Count; i++)
        {
            scorePoints[i] += data.CharacterScore[BetweenScenePass.Games.Sacks][i];
        }
        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Chairs].Count; i++)
        {
            scorePoints[i] += data.CharacterScore[BetweenScenePass.Games.Chairs][i];
        }
        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Jousting].Count; i++)
        {
            scorePoints[i] += data.CharacterScore[BetweenScenePass.Games.Jousting][i];
        }
        for (int i = 0; i < data.CharacterScore[BetweenScenePass.Games.Baloons].Count; i++)
        {
            scorePoints[i] += data.CharacterScore[BetweenScenePass.Games.Baloons][i];
        }

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < scorePoints[i]; j++)
            {
                if (scores[i].transform.childCount >= j) {
                    scores[i].transform.GetChild(j).gameObject.SetActive(true);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
