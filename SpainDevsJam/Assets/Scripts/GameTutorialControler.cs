using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
[System.Serializable] public class TutorialClose : UnityEvent { }
public class GameTutorialControler : MonoBehaviour
{
    [Space(10)]
    [SerializeField] CMCameraRail cinematic;
    [SerializeField] GameObject initCinematic;
    [SerializeField] GameObject mainCamera;

    [Space(10)]
    [SerializeField] GameObject cardboard;

    [Space(10)]
    [SerializeField] string gameName;
    [SerializeField] TextMeshProUGUI gameNameText;

    [Space(10)]
    [SerializeField] string gameDescription;
    [SerializeField] TextMeshProUGUI gameDescriptionText;

    [Space(10)]
    [SerializeField] GameObject startButton;

    [Space(10)]
    [SerializeField] GameObject controlsPanel;

    [Space(10)]
    [SerializeField] GameObject videoPanel;
    [SerializeField] VideoClip video;

    [Space(10)]
    [SerializeField] Animator anim;


    [Space]
    public TutorialClose onTutorialClose;


    CursorLockMode cursorLastState;

    public bool gameStarted;


    void Awake()
    {
        if(mainCamera != null)
            mainCamera.SetActive(false);

        gameNameText.text = gameName;
        gameDescriptionText.text = gameDescription;

        cinematic.StartRail();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameStarted = false;

        StartCoroutine(AudioManager.instance.FadeIn("transicion", 1));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartClick()
    {
        Time.timeScale = 1;
        gameStarted = true;
        anim.SetTrigger("hide");

        AudioManager.instance.PlayOneShot("SFX_click");

        if (mainCamera != null)
            mainCamera.SetActive(true);

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

        if (onTutorialClose.GetPersistentEventCount() > 0)
        {
            onTutorialClose.Invoke();
        }

    }

    public void ShowTutorial()
    {
        anim.SetTrigger("show");
        initCinematic.SetActive(false);

        cursorLastState = Cursor.lockState;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
}
