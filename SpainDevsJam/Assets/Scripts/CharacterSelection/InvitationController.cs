
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class InvitationController : MonoBehaviour
{

    [SerializeField] ScenesTransitionController sceneTransition;
    [SerializeField] GameTutorialControler tutorial;

    [Space(20)]
    [SerializeField] float showDelay;
    [SerializeField] float hideDelay;

    [Header("Invitation Panels")]
    [SerializeField] GameObject leftPanelBack;
    [SerializeField] GameObject leftPanel;
    [SerializeField] GameObject rightPanel;

    [Header("Menu Panels")]
    [SerializeField] GameObject title;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject controls;
    [SerializeField] GameObject options;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject confirmationRight;
    [SerializeField] GameObject confirmationLeft;

    [Space(10)]
    [SerializeField] TextMeshProUGUI confirmationQuestionRight;
    [SerializeField] TextMeshProUGUI confirmationQuestionLeft;


    [Space(10)]
    [SerializeField] Animator anim;

    [Space(10)]
    [SerializeField] bool mainMenuOnBackClick;

    [Header("Player Selector Objects (MAIN MENU ONLY)")]
    [SerializeField] GameObject characterSelectorController;
    [SerializeField] GameObject characters;
    [SerializeField] GameObject npcs;
    [SerializeField] GameObject panelPhrase;
    [SerializeField] GameObject nextCharButton;
    [SerializeField] GameObject prevCharButton;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject backMenuButton;

    [SerializeField] bool pauseIsUp;

    [SerializeField] string confirmTrigger;

    [SerializeField] GameObject lastShowedPanel;

    CursorLockMode cursorLastState;


    void Awake()
    {

        if (GameObject.Find("BetweenScenePass") != null)
            StartCoroutine(StartInvitation());
        else
        {
            MenuShowConfig();
            options.SetActive(false);
        }
    }

    IEnumerator StartInvitation()
    {
        yield return new WaitUntil(() => BetweenScenePass.instance != null);
        MenuShowConfig();
        options.SetActive(false);
    }

    void MenuShowConfig()
    {
        if (BetweenScenePass.instance && !BetweenScenePass.instance.GameRestarted && mainMenuOnBackClick)
            mainMenuOnBackClick = false;

        if (mainMenuOnBackClick)
        {
            title.SetActive(true);
            mainMenu.SetActive(true);

            leftPanelBack.SetActive(true);
            leftPanel.SetActive(true);
            rightPanel.SetActive(true);

            lastShowedPanel = mainMenu;

            Invoke("HideTitle", 17.5f);
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "EndGame")
                backMenuButton.SetActive(false);

            pauseMenu.SetActive(true);
            anim.SetBool("iddle", true);
            lastShowedPanel = pauseMenu;
        }

        if (backMenuButton != null)
        {
            backMenuButton.SetActive(false);
        }
    }

    void HideTitle()
    {
        title.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (tutorial == null || tutorial.gameStarted)
        {
            if (!mainMenuOnBackClick && Input.GetKeyDown(KeyCode.Escape) && (characterSelectorController == null || !characterSelectorController.activeInHierarchy))
            {
                if (pauseIsUp)
                {
                    Time.timeScale = 1;
                    anim.SetTrigger("pauseDown");
                    Invoke("ReactivatePauseMenuAfterGoDown", 0.2f);

                    if (cursorLastState == CursorLockMode.Locked)
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
                else
                {
                    Time.timeScale = 0;
                    anim.SetTrigger("pauseUp");

                    cursorLastState = Cursor.lockState;

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                pauseIsUp = !pauseIsUp;
            }

        }
    }

    void ReactivatePauseMenuAfterGoDown()
    {
        pauseMenu.SetActive(true);
        controls.SetActive(false);
        options.SetActive(false);
        credits.SetActive(false);
        confirmationRight.SetActive(false);
        confirmationLeft.SetActive(false);
        mainMenu.SetActive(false);

        lastShowedPanel = pauseMenu;

        StopAllCoroutines();
    }

    public void OnPlayClick()
    {
        mainMenuOnBackClick = false;
        anim.SetTrigger("pauseDown");
        pauseIsUp = false;

        Invoke("ReactivatePauseMenuAfterGoDown", 0.2f);

        characterSelectorController.SetActive(true);
        characters.SetActive(true);
        npcs.transform.position = new Vector3(npcs.transform.position.x, -10, npcs.transform.position.z);
        panelPhrase.SetActive(true);
        nextCharButton.SetActive(true);
        prevCharButton.SetActive(true);
        playButton.SetActive(true);
    }

    public void OnResumeClick()
    {
        Time.timeScale = 1;
        anim.SetTrigger("pauseDown");
        pauseIsUp = false;

        if (cursorLastState == CursorLockMode.Locked)
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

    public void OnControlsClick()
    {
        StartCoroutine(HidePanel(lastShowedPanel));
        StartCoroutine(ShowPanel(controls));
        anim.SetTrigger("rotate");
    }
    
    public void OnOptionsClick()
    {
        StartCoroutine(HidePanel(lastShowedPanel));
        StartCoroutine(ShowPanel(options));
        anim.SetTrigger("rotate");
    }
    
    public void OnCreditsClick()
    {
        StartCoroutine(HidePanel(lastShowedPanel));
        StartCoroutine(ShowPanel(credits));
        anim.SetTrigger("rotate");
    }
   
    public void OnExitClick()
    {
        confirmationQuestionLeft.text = "¿Estás seguro de que deseas salir?";
        confirmTrigger = "EXIT";

        StartCoroutine(HidePanel(lastShowedPanel));
        StartCoroutine(ShowPanel(confirmationLeft));
        anim.SetTrigger("rotate");
    }

    public void OnExitClickDirectly()
    {
        Application.Quit();
    }

    public void OnExitMenuClick()
    {
        confirmationQuestionLeft.text = "¿Estás seguro de que deseas reiniciar el juego?\r\nTodo el progreso se perderá.";
        confirmTrigger = "RESTART";
        StartCoroutine(AudioManager.instance.FadeOut("01_jardin", 1));
        StartCoroutine(HidePanel(lastShowedPanel));
        StartCoroutine(ShowPanel(confirmationLeft));
        anim.SetTrigger("rotate");
    }
    public void OnExitMenuClickDirectly()
    {
        AudioManager.instance.StopAll();
        BetweenScenePass.instance.GameRestarted = true;
        BetweenScenePass.instance.RestartData();
        sceneTransition.CloseScene("Scenes/Menu");
    }

    public void OnBacktMenuClick()
    {
        confirmationQuestionLeft.text = "¿Estás seguro de que deseas volver al jardín?";
        confirmTrigger = "BACK_MENU";

        StartCoroutine(HidePanel(lastShowedPanel));
        StartCoroutine(ShowPanel(confirmationLeft));
        anim.SetTrigger("rotate");
    }

    public void OnBackClick()
    {
        StartCoroutine(HidePanel(lastShowedPanel));

        if (mainMenuOnBackClick)
        {
            StartCoroutine(ShowPanel(mainMenu));
        }
        else
        {
            StartCoroutine(ShowPanel(pauseMenu));
        }

        anim.SetTrigger("rotate");
    }

    public void OnConfirmAccept()
    {
        switch (confirmTrigger)
        {
            case "EXIT":
                Application.Quit();
                break;
            case "RESTART":
                Time.timeScale = 1;
                BetweenScenePass.instance.GameRestarted = true;
                BetweenScenePass.instance.RestartData();
                AudioManager.instance.StopAll();
                sceneTransition.CloseScene("Scenes/Menu");
                break;
            case "BACK_MENU":
                Time.timeScale = 1;
                BetweenScenePass.instance.GameRestarted = false;
                AudioManager.instance.StopAll();
                sceneTransition.CloseScene("Scenes/Menu");
                break;
            case "SELECT_CHAR":
                characterSelectorController.GetComponent<CharSelSelectionController>().ConfirmPlay();
                anim.SetTrigger("pauseDown");
                Invoke("ReactivatePauseMenuAfterGoDown", 0.2f);
                break;
        }
        confirmTrigger = "";
    }

    public void OnConfirmCancel()
    {
        switch (confirmTrigger)
        {
            case "SELECT_CHAR":
                nextCharButton.SetActive(true);
                prevCharButton.SetActive(true);
                playButton.SetActive(true);

                anim.SetTrigger("pauseDown");
                Invoke("ReactivatePauseMenuAfterGoDown", 0.2f);
                break;
            default:
                OnBackClick();
                break;
        }

        confirmTrigger = "";

    }

    public void OpensConfirmDirectly(string charName)
    {
        nextCharButton.SetActive(false);
        prevCharButton.SetActive(false);
        playButton.SetActive(false);

        anim.SetTrigger("pauseUp");
        confirmationQuestionRight.text = "Quieres jugar con "+ charName + "? ";
        pauseMenu.SetActive(false);
        confirmationRight.SetActive(true);
        confirmTrigger = "SELECT_CHAR";
    }

    
    IEnumerator ShowPanel(GameObject panel)
    {
        yield return new WaitForSecondsRealtime(showDelay);
        panel.SetActive(true);
        lastShowedPanel = panel;
    }

    IEnumerator HidePanel(GameObject panel)
    {
        yield return new WaitForSecondsRealtime(hideDelay);
        panel.SetActive(false);
    }

}
