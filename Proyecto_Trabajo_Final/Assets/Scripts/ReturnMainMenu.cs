using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMainMenu : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerController.m_HasTriggeredBossFight = false;
            Close();
        }
    }
    public void Close()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
