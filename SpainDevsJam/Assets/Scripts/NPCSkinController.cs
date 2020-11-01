using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCSkinController : MonoBehaviour
{

    [SerializeField] List<CharacterIdController> npcs;

    // Start is called before the first frame update
    void Start()
    {
        BetweenScenePass data = BetweenScenePass.instance;
        int npcCount = npcs.Count;

        if (data && data.NpcCharacterId.Count > 0) {

            data.NpcCharacterId.Sort((a, b) => 1 - 2 * Random.Range(0, 1));

            for (int i = 0; i < data.NpcCharacterId.Count; i++)
            {
                Animator modelAnim = npcs[i].ActivateModel(data.NpcCharacterId[i]);

                if (SceneManager.GetActiveScene().name == "Sillas")
                    npcs[i].gameObject.GetComponent<ChairsEnemyController>().anim = modelAnim;

                if (SceneManager.GetActiveScene().name == "CarreraSacos")
                    npcs[i].gameObject.GetComponent<SaltosSacosController>().anim = modelAnim;

                if (SceneManager.GetActiveScene().name == "Justa")
                {
                    if(npcs[i].gameObject.GetComponent<JustaPlayerController>())
                        npcs[i].gameObject.GetComponent<JustaPlayerController>().anim = modelAnim;

                    if (!npcs[i].gameObject.CompareTag("Enemy"))
                        npcs[i].DanceDance(Random.Range(0, 5));
                }


            }

        }
        else
        {
            for(int i = 0; i < npcCount; i++)
            {
                Animator modelAnim = npcs[i].ActivateModel(i);

                if (SceneManager.GetActiveScene().name == "Sillas")
                    npcs[i].gameObject.GetComponent<ChairsEnemyController>().anim = modelAnim;

                if (SceneManager.GetActiveScene().name == "CarreraSacos")
                    npcs[i].gameObject.GetComponent<SaltosSacosController>().anim = modelAnim;

                if (SceneManager.GetActiveScene().name == "Justa")
                {
                    if(npcs[i].gameObject.GetComponent<JustaPlayerController>())
                        npcs[i].gameObject.GetComponent<JustaPlayerController>().anim = modelAnim;

                    if (!npcs[i].gameObject.CompareTag("Enemy"))
                        npcs[i].DanceDance(Random.Range(0, 5));
                }
            }
        }

    }
}
