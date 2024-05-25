using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGhostScript : EnemyScript
{
    private CapsuleCollider2D m_Collider;
    private Rigidbody2D m_Rigidbody2D;
    public Transform[] m_PatrolPoints;
    private Animator m_Animator;
    public GameObject m_DeathParticlesPrefab;

    public enum BOSS_GHOST_BEHAVIOUR
    {
        IDLE,
        PATROL_POINT, 
        FOLLOW_PLAYER
    }
    public BOSS_GHOST_BEHAVIOUR m_BossGhostBehaviour = BOSS_GHOST_BEHAVIOUR.IDLE;

    // Variables
    public float m_BossGhostMaxSpeed = 2;
    public float m_BossGhostCurrentSpeed;
    public float m_KnockbackForce;

    private int m_CurrentPatrolPointIndex = 0;
    private bool m_CanMove;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentLifePoints = m_MaxLifePoints;
        m_BossGhostCurrentSpeed = m_BossGhostMaxSpeed;
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Collider = GetComponent<CapsuleCollider2D>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Animator = GetComponentInChildren<Animator>();
        m_CanMove = true;

        InvokeRepeating("ScarePlayer", 3, 5);
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        switch (m_BossGhostBehaviour)
        {
            case BOSS_GHOST_BEHAVIOUR.PATROL_POINT:
                PatrolPoints(dt);
                break;
            
            case BOSS_GHOST_BEHAVIOUR.FOLLOW_PLAYER:
                FollowPlayer(dt);
                break;
        }

        if (m_CurrentLifePoints <= 0)
        {
            m_CanMove = false;
            m_Collider.enabled = false;
            m_Animator.SetTrigger("Dead");
        }

        // Invoke repeatedly after a certain amount of time the function to scare the player
    }

    private void PatrolPoints(float dt)
    {
        MoveToPoint(dt);
    }

    private void MoveToPoint(float dt)
    {
        if (!m_CanMove) return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = m_PatrolPoints[m_CurrentPatrolPointIndex].position;

        transform.position = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            m_BossGhostCurrentSpeed * dt);
        float stopDistance = 0.1f;

        stopDistance = Mathf.Pow(stopDistance, 2);
        float xElement = Mathf.Pow(targetPosition.x - currentPosition.x, 2);
        float yElement = Mathf.Pow(targetPosition.y - currentPosition.y, 2);

        if (xElement + yElement < stopDistance)
        {
            SelectNextPoint();
        }

        CheckIfFlipNeeded();
    }

    void FollowPlayer(float dt)
    {
        if (!m_CanMove) return;

        Vector2 direction = m_Player.transform.position - transform.position;

        //Debug.Log(direction.x);
        transform.position = Vector2.MoveTowards(
            this.transform.position,
            m_Player.transform.position,
            (m_BossGhostCurrentSpeed * 0.75f) * dt);
        

        if (direction.x < 0)
        {
            if (GoingRight)
            {
                GoingRight = !GoingRight;
            }
        }
        else
        {
            if (!GoingRight)
            {
                GoingRight = !GoingRight;
            }
        }
    }


    private void CheckIfFlipNeeded()
    {
        Vector2 curentPoint = m_PatrolPoints[m_CurrentPatrolPointIndex].position;
        if (curentPoint.x < transform.position.x)
        {
            if (GoingRight)
            {
                GoingRight = !GoingRight;
            }
        }
        else
        {
            if (!GoingRight)
            {
                GoingRight = !GoingRight;
            }
        }
    }

    private void SelectNextPoint()
    {
        m_CurrentPatrolPointIndex++;
        if (m_CurrentPatrolPointIndex >= m_PatrolPoints.Length)
        {
            m_CurrentPatrolPointIndex = 0;
        }
    }

    public override void GetDamage(int howMuchDamage)
    {
        base.GetDamage(howMuchDamage);
    }


    private void ScarePlayer()
    {
        if (m_BossGhostBehaviour == BOSS_GHOST_BEHAVIOUR.IDLE) return;

        m_Animator.SetTrigger("Scare");

        // Change behaviour from point to follow and viceversa each time it scares the player
        if (m_BossGhostBehaviour == BOSS_GHOST_BEHAVIOUR.PATROL_POINT)
        {
            m_BossGhostBehaviour = BOSS_GHOST_BEHAVIOUR.FOLLOW_PLAYER;
        }
        else
        {
            m_BossGhostBehaviour = BOSS_GHOST_BEHAVIOUR.PATROL_POINT;
        }
    }

    public void ToggleCanMove()
    {
        m_CanMove = !m_CanMove;
    }

    public void DestroyBoss()
    {
        Instantiate(m_DeathParticlesPrefab, transform.position, Quaternion.Euler(0, 0, 90));  
        Destroy(gameObject, 1);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Lamp")
        {
            // Deal knockback to the ghost
            if (collision.transform.position.x < transform.position.x)
            {
                m_Rigidbody2D.AddForce((Vector2.right + Vector2.up) * m_KnockbackForce / 2);
            }
            else
            {
                m_Rigidbody2D.AddForce((Vector2.left + Vector2.up) * m_KnockbackForce / 2);
            }
        }
    }
}
