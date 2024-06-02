using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreen : MonoBehaviour
{
    public GameObject m_Panel;
    public Material m_Material;
    public Color[] m_Colors;
    PlayerController m_PlayerController;
    private int m_CurrentColorIndex;
    private int m_TargetColorIndex;
    private float m_TargetPoint;
    public float m_Time;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Panel");
        m_PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if(m_PlayerController.m_IsDead == true)
        {
            Darker();
        }
    }

    private void Darker()
    {
        m_TargetPoint += Time.deltaTime / m_Time;
        m_Material.color = Color.Lerp(m_Colors[m_CurrentColorIndex], m_Colors[m_TargetColorIndex], m_TargetPoint);
        if(m_TargetPoint >= 1f)
        {
            m_TargetPoint = 0f;
            m_CurrentColorIndex = m_TargetColorIndex;
            m_TargetColorIndex++;
            if (m_TargetColorIndex == m_Colors.Length)
                m_TargetColorIndex = 0;
        }
    }

    private void Lighter()
    {
        
    }
}
