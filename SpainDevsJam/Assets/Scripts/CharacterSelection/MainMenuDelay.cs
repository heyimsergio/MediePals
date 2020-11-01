using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenuDelay : MonoBehaviour{

    [SerializeField] float delay;
    [SerializeField] GameObject invitation;

    [SerializeField] CMCameraRail cinematic;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject mainCam;
    [SerializeField] GameObject track;
    [SerializeField] GameObject player;

    // Start is called before the first frame update
    void Awake(){
        StartCoroutine(StartCameraController());
    }

    private void Start()
    {
    }

    IEnumerator StartCameraController()
    {

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        yield return new WaitUntil(() => BetweenScenePass.instance != null);

        if (BetweenScenePass.instance && BetweenScenePass.instance.GameRestarted)
        {
            AudioManager.instance.Play("transicion", 0f);
            AudioManager.instance.Play("SFX_vocesBackground");

            cam.SetActive(true);
            track.SetActive(true);

            player.SetActive(false);
            cinematic.StartRail();
            StartCoroutine(ActivateMenu(false));
        }
        else
        {
            AudioManager.instance.Play("01_jardin", 0f);

            player.transform.position = new Vector3(0f, 1.08f, -7.68f);

            track.SetActive(false);

            player.SetActive(true);

            cam.SetActive(false);
            mainCam.SetActive(true);
            mainCam.GetComponent<CinemachineFreeLook>().m_YAxis.Value = 0.5f;
            mainCam.GetComponent<CinemachineFreeLook>().m_XAxis.Value = -180;

            GameObject selectedNPC;
            if (BetweenScenePass.instance.PlayerCharacterId >= 0)
            {
                selectedNPC = GameObject.Find("LevelNPC_" + BetweenScenePass.instance.PlayerCharacterId);
            }
            else
            {
                selectedNPC = GameObject.Find("LevelNPC_0");
            }

            player.GetComponent<MenuPlayerMovement>().anim = player.GetComponent<CharacterIdController>().SetPlayerCustom();
            player.GetComponent<MenuPlayerMovement>().anim.SetFloat("Dance", Random.Range(1f, 6f));

            //player.transform.position = new Vector3(selectedNPC.transform.position.x, 1.08f, selectedNPC.transform.position.z);
            //player.transform.rotation = selectedNPC.transform.rotation;

            selectedNPC.SetActive(false);

            delay = 0;
            StartCoroutine(ActivateMenu(true));
        }
    }

    IEnumerator ActivateMenu(bool lockMouse){
        yield return new WaitForSecondsRealtime(delay);
        StartCoroutine(AudioManager.instance.FadeOut("transicion", 1));
        StartCoroutine(AudioManager.instance.FadeIn("01_jardin", 1));
        invitation.GetComponent<Animator>().SetTrigger("startMainMenu");

        if (lockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
