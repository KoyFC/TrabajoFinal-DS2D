using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossGhostScript;

public class BossHealthBarScript : MonoBehaviour
{

    private GameObject m_Player;
    public GameObject m_HealthBar;

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (m_Player.GetComponent<PlayerController>().m_ActivateBossFight)
        {
            m_HealthBar.SetActive(true);
        }
    }
}
