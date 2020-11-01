using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject exitPanel;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Options()
    {
        optionsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void Controls()
    {
        controlsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void Credits()
    {
        creditsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void Back()
    {
        if (creditsPanel.activeSelf)
        {
            creditsPanel.SetActive(false);
        }
        if (optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
        }
        if (controlsPanel.activeSelf)
        {
            controlsPanel.SetActive(false);
        }
        if (exitPanel.activeSelf)
        {
            exitPanel.SetActive(false);
        }
        mainPanel.SetActive(true);
    }

    public void Exit()
    {
        exitPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        //Change scene
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
