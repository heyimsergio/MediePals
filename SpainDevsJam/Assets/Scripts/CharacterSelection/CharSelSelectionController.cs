using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class CharSelSelectionController : MonoBehaviour
{
    [Space(10)]
    [SerializeField] GameObject levelNPCs;
    [SerializeField] List<GameObject> npcList;

    [Space(10)]
    [SerializeField] List<GameObject> characterList;
    [SerializeField] GameObject selectedCharacter;

    [Space(10)]
    [SerializeField] Transform spawnPointLeft;
    [SerializeField] Transform spawnPointRight;

    [Space(10)]
    [SerializeField] TextMeshProUGUI phraseText;
    [SerializeField] GameObject phrasePanel;
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject prevButton;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject characters;
    [SerializeField] GameObject player;
    [SerializeField] GameObject trackCamera;
    [SerializeField] GameObject cam;

    int selectedCharacterIndex = 0;

    bool canSpawnCharacter = true;

    
    void Awake()
    {
        selectedCharacter = characterList[selectedCharacterIndex];
        phraseText.text = selectedCharacter.GetComponent<CharSelCharacterController>().phrase;
    }

    public void OnPlayClick()
    {
        if (canSpawnCharacter)
        {
            GameObject.Find("Menu").GetComponent<InvitationController>().OpensConfirmDirectly(selectedCharacter.GetComponent<CharSelCharacterController>().charName);
        }
    }

    public void ConfirmPlay()
    {
        BetweenScenePass data = BetweenScenePass.instance;
        CharacterIdController selecterCharacterCustom = selectedCharacter.GetComponent<CharacterIdController>();

        phrasePanel.SetActive(false);
        nextButton.SetActive(false);
        prevButton.SetActive(false);
        playButton.SetActive(false);
        characters.SetActive(false);
        levelNPCs.transform.position = new Vector3(levelNPCs.transform.position.x, 0, levelNPCs.transform.position.z);
        trackCamera.SetActive(false);

        int selectedCharId = selecterCharacterCustom.CharacterId;

        data.PlayerCharacterId = selectedCharId;

        GameObject selectedNPC = null;

        for (int i = 0; i < npcList.Count; i++)
        {
            if(npcList[i].GetComponent<CharacterIdController>().CharacterId == selectedCharId)
            {
                selectedNPC = npcList[i];
            }
            else
            {
                data.NpcCharacterId.Add(npcList[i].GetComponent<CharacterIdController>().CharacterId);
            }
        }

        player.GetComponent<MenuPlayerMovement>().anim = player.GetComponent<CharacterIdController>().SetPlayerCustom();
        

        player.transform.position = new Vector3(selectedNPC.transform.position.x, 1.08f, selectedNPC.transform.position.z);
        player.transform.rotation = selectedNPC.transform.rotation;

        selectedNPC.SetActive(false);

        player.SetActive(true);


        cam.SetActive(true);


        gameObject.SetActive(false);


    }

    public void OnPrevCharacterClick()
    {
        if (canSpawnCharacter && (phrasePanel.transform.localScale.x > 0.5 || phrasePanel.transform.localScale.x < 0.5))
        {
            canSpawnCharacter = false;
            StartCoroutine(MoveCharacter(0));
        }
    }

    public void OnNextCharacterClick()
    {
        if (canSpawnCharacter && (phrasePanel.transform.localScale.x > 0.5 || phrasePanel.transform.localScale.x < 0.5))
        {
            canSpawnCharacter = false;
            StartCoroutine(MoveCharacter(1));
        }
    }


    void UpdateIndex(int direction)
    {
        if (selectedCharacterIndex == 0 && direction == 0)
        {
            selectedCharacterIndex = characterList.Count - 1;
        }else if (selectedCharacterIndex == characterList.Count - 1 && direction == 1)
        {
            selectedCharacterIndex = 0;
        }
        else
        {
            selectedCharacterIndex += direction == 0 ? -1 : 1;
        }
    }

    void UpdateSelectedCharacter(float posX)
    {
        selectedCharacter = characterList[selectedCharacterIndex];
        selectedCharacter.transform.localPosition =  new Vector3(posX, 0, selectedCharacter.transform.localPosition.z);
        selectedCharacter.SetActive(true);
    }

    void EmptyCharacterPhrase()
    {
        phraseText.text = " ";
        phrasePanel.GetComponent<Animator>().SetBool("visible", false);
    }

    IEnumerator MoveCharacter(int direction)
    {
        EmptyCharacterPhrase();

        yield return new WaitForSeconds(0.15f);

        if (direction == 0)
            selectedCharacter.GetComponent<CharSelCharacterController>().WalkLeft(false);
        else if(direction == 1)
            selectedCharacter.GetComponent<CharSelCharacterController>().WalkRight(false);

        yield return new WaitForSeconds(0.2f);

        UpdateIndex(direction);

        if (direction == 0)
        {
            UpdateSelectedCharacter(spawnPointRight.transform.localPosition.x);
            selectedCharacter.GetComponent<CharSelCharacterController>().WalkLeft(true);
        }
        else if (direction == 1)
        {
            UpdateSelectedCharacter(spawnPointLeft.transform.localPosition.x);
            selectedCharacter.GetComponent<CharSelCharacterController>().WalkRight(true);
        }

        yield return new WaitForSeconds(0.5f);

        canSpawnCharacter = true;
    }

}
