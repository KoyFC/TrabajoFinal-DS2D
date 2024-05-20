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
    private int m_CurrentFlameIndex;
    private int i;

    void Update()
    {
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
    }

    IEnumerator DeactivateFlame(int index, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        m_Flames[index].SetActive(false);
    }
}
