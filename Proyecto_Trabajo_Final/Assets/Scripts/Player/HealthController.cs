using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public PlayerController m_PlayerController;
    public GameObject[] m_Flames;
    public Color[] m_Colors;
    public Animator[] m_FlameAnimators;
    private int i;

    void Update()
    {
        // Update the flames based on the player's life points. Activate or deactivate them if the player's health is 5 or lower. Change their color if it's higher.
        for (i = 0; i < m_Flames.Length; i++)
        {
            if (m_PlayerController.m_LifePoints <= i)
            {
                m_FlameAnimators[i].SetTrigger("Damaged");
                StartCoroutine(DeactivateFlame(i, 0.25f));
            }
            else
            {
                m_Flames[i].SetActive(true);
            }
        }

        if (m_PlayerController.m_LifePoints > 5)
        {
            for (i = m_Flames.Length; i < m_PlayerController.m_LifePoints; i++)
            {
                m_Flames[i - m_Flames.Length].GetComponent<Image>().color = m_Colors[1];
            }

            
        }
        else if (m_PlayerController.m_LifePoints <= 5)
            {
                for (i = 0; i < m_Flames.Length; i++)
                {
                    m_Flames[i].GetComponent<Image>().color = m_Colors[0];
                }
            }
    }

    IEnumerator DeactivateFlame(int index, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        m_Flames[index].SetActive(false);
    }
}
