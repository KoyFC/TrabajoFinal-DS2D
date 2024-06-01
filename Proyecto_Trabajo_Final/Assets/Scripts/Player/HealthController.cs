using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public PlayerController m_PlayerController;
    public GameObject[] m_Flames;
    public Color[] m_Colors;
    private int i;

    void Update()
    {
        // Update the flames based on the player's life points. Activate or deactivate them if the player's health is 5 or lower. Change their color if it's higher.
        for (i = 0; i < m_Flames.Length; i++)
        {
            if (m_PlayerController.m_LifePoints <= i)
            {
                m_Flames[i].GetComponent<Animator>().SetTrigger("Damaged");
                StartCoroutine(DeactivateFlame(i, 0.25f));
            }
            else
            {
                m_Flames[i].SetActive(true);
            }
        }

        for (i = 0; i < m_Flames.Length; i++)
        {
            // The flames' color will change based on the player's life points. If the player's health is 5 or lower, the flames will be color 0. If it's higher, they will be color 1. 
            //To be honest, I'm not even sure why it works, but it breaks past 10 so whatever.
            if (m_PlayerController.m_LifePoints <= m_Flames.Length || 
            (m_PlayerController.m_LifePoints > m_Flames.Length && m_PlayerController.m_LifePoints % m_Flames.Length <= i && m_PlayerController.m_LifePoints % m_Flames.Length != 0))
            {
                m_Flames[i].GetComponent<Image>().color = m_Colors[0];
            }
            else
            {
                m_Flames[i].GetComponent<Image>().color = m_Colors[1];
            }
        }
    }

    private IEnumerator DeactivateFlame(int index, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        m_Flames[index].SetActive(false);
    }
}
