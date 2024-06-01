using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProyectileScript : MonoBehaviour
{
    private GameObject m_Player;
    public int m_LifePoints = 1;
    public int m_DamageDealtToPlayer = 2;
    public float m_ProyectileSpeed;
    private Vector2 m_TargetPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_TargetPosition = m_Player.transform.position;
        Destroy(gameObject, 6);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_LifePoints <= 0)
        {
            Destroy(gameObject);
        }
        FireToPlayer();
    }

    private void FireToPlayer()
    {
        Vector2 currentPosition = transform.position;

        transform.position = Vector2.MoveTowards(
            currentPosition,
            m_TargetPosition,
            m_ProyectileSpeed * Time.deltaTime);
        if (currentPosition == m_TargetPosition)
        {
            m_TargetPosition = m_Player.transform.position;
        }
    }

    public void GetDamage(int damage)
    {
        if (m_LifePoints > 0)
        {
            m_LifePoints -= damage;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, 0.15f);
        }
    }
}
