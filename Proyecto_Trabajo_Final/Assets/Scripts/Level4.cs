using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level4 : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }
}
