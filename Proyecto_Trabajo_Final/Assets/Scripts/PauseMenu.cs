using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseButton;
    public GameObject PauseMenuO;
    private bool GamePaused = false;

    private void Start()
    {
        Resume();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GamePaused)
            {
                Reset();
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (GamePaused)
            {
                Close();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GamePaused)
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0f;
        PauseButton.SetActive(false);
        PauseMenuO.SetActive(true);
    }

    public void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1f;
        PauseButton.SetActive(true);
        PauseMenuO.SetActive(false);
    }

    public void Reset()
    {
        GamePaused = false;
        Time.timeScale = 1f;
        PlayerController.m_HasTriggeredBossFight = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Close()
    {
        PlayerController.m_HasTriggeredBossFight = false;
        SceneManager.LoadScene("TitleScreen");
    }
}
    
