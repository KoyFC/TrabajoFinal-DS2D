using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject m_Player;
    public LightDamageScript m_LightDamageScript;
    public int m_CurrentLifePoints;
    public int m_MaxLifePoints = 3;
    public int m_DamageDealtToPlayer = 1;
    public Vector3 m_SpawnPoint;

    public bool m_GoingRight;
    public bool GoingRight
    {
        get
        {
            return m_GoingRight;
        }
        set
        {
            if (m_GoingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            m_GoingRight = value;
        }
    }

    private void Start()
    {
        // ADD EVERYTHING IN HERE INSIDE SCRIPTS THAT INHERIT FROM THIS SCRIPT
        m_CurrentLifePoints = m_MaxLifePoints;
        m_SpawnPoint = transform.position;
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_LightDamageScript = m_Player.GetComponentInChildren<LightDamageScript>();
    }

    public virtual void GetDamage(int damage)
    {
        if (m_CurrentLifePoints > 0)
        {
            m_CurrentLifePoints--;
        }
    }
}
