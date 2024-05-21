using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2 : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Level2");
    }

    public void Salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }
}
