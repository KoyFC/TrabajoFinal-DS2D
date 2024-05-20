using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int m_CurrentLifePoints;
    public int m_MaxLifePoints = 3;
    public int m_DamageDealtToPlayer = 1;
    private Vector2 m_SpawnPoint;

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
        m_CurrentLifePoints = m_MaxLifePoints;
        m_SpawnPoint = transform.position;
    }

    public virtual void GetDamage(int damage)
    {
        if (m_CurrentLifePoints > 0)
        {
            m_CurrentLifePoints--;
        }
    }

    public virtual void Respawn()
    {
        m_CurrentLifePoints = m_MaxLifePoints;
        transform.position = m_SpawnPoint;
        gameObject.SetActive(true);
    }
}
