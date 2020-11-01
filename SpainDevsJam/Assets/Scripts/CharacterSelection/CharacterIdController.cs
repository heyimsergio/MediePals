using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterIdController : MonoBehaviour
{
    public int characterId;
    [SerializeField] int dance;
    [SerializeField] Animator anim;

    [SerializeField] List<GameObject> models;

    public int CharacterId { get => characterId; set => characterId = value; }
    public int Dance { get => dance; set => dance = value; }

    private void Awake()
    {
        if(gameObject.CompareTag("Player"))
            anim = GetComponent<CharacterIdController>().SetPlayerCustom();

        if (characterId > -1)
        {
            if (Dance > 0)
            {
                DanceDance(Dance);
            }

        }
    }

    public void DanceDance(int _dance)
    {
        Dance = _dance;
        anim.SetFloat("Dance", Dance);
    }

    public Animator SetRandomNpcCustom()
    {
        BetweenScenePass data = BetweenScenePass.instance;

        if (data && data.NpcCharacterId.Count > 0)
        {
            characterId = data.NpcCharacterId[Random.Range(0, data.NpcCharacterId.Count-1)];
        }
        else
        {
            characterId = 5;
        }

        return ActivateModel(characterId);
    }

    public Animator SetNpcCustom(int npcId)
    {
        BetweenScenePass data = BetweenScenePass.instance;

        if (data && data.NpcCharacterId.Count > 0)
        {
            if (data.NpcCharacterId.Contains(npcId))
                characterId = data.NpcCharacterId[data.NpcCharacterId.IndexOf(npcId)];
            else if (npcId == data.PlayerCharacterId)
                characterId = data.PlayerCharacterId;
        }
        else
        {
            characterId = 5;
        }

        return ActivateModel(npcId);
    }

    public Animator ActivateModel(int npcId)
    {

        characterId = npcId;

        models[npcId].SetActive(true);

        anim = models[npcId].GetComponent<Animator>();

        return models[npcId].GetComponent<Animator>();
    }


    public Animator SetPlayerCustom()
    {
        BetweenScenePass data = BetweenScenePass.instance;

        if (data && data.PlayerCharacterId > -1)
        {
            characterId = data.PlayerCharacterId;
        }
        else
        {
            characterId = 0;
        }

        return ActivateModel(characterId);
    }
}
