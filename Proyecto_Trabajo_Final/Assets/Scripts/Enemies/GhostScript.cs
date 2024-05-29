using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : EnemyScript
{
    private CapsuleCollider2D m_Collider;
    private Rigidbody2D m_Rigidbody2D;
    public Transform[] m_PatrolPoints;
    private Animator m_Animator;

    public enum GHOST_BEHAVIOUR
    {
        IDLE,
        PATROL_POINT, 
        FOLLOW_PLAYER
    }
    public GHOST_BEHAVIOUR m_GhostBehaviour = GHOST_BEHAVIOUR.IDLE;

    // Variables
    public float m_GhostMaxSpeed = 2;
    public float m_GhostCurrentSpeed;
    public Vector2 m_FollowDistance;
    private Vector2 m_Direction;

    private int m_CurrentPatrolPointIndex = 0;
    private bool m_CanMove;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentLifePoints = m_MaxLifePoints;
        m_GhostCurrentSpeed = m_GhostMaxSpeed;
        m_GoingRight = false;
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Collider = GetComponentInChildren<CapsuleCollider2D>();
        m_Rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        m_Animator = GetComponentInChildren<Animator>();
        m_CanMove = true;

        if (m_GhostBehaviour == GHOST_BEHAVIOUR.PATROL_POINT)
        {
            CheckIfFlipNeeded();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        m_Direction = m_Player.transform.position - transform.position;
        if (m_FollowDistance.x > Mathf.Abs(m_Direction.x) && m_FollowDistance.y > Mathf.Abs(m_Direction.y))
        {
            m_GhostBehaviour = GHOST_BEHAVIOUR.FOLLOW_PLAYER;
        }
        else
        {
            m_GhostBehaviour = GHOST_BEHAVIOUR.IDLE;
        }

        switch (m_GhostBehaviour)
        {
            case GHOST_BEHAVIOUR.PATROL_POINT:
                PatrolPoints(dt);
                break;
            
            case GHOST_BEHAVIOUR.FOLLOW_PLAYER:
                FollowPlayer(dt);
                break;
        }
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
            m_GhostCurrentSpeed * dt);
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

        //Debug.Log(m_Direction.x);
        transform.position = Vector2.MoveTowards(
            this.transform.position,
            m_Player.transform.position,
            (m_GhostCurrentSpeed * 0.5f) * dt);

        if (m_Direction.x < 0)
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

    public void ToggleCanMove()
    {
        m_CanMove = false;
        Invoke("CanMoveAgain", 0.4f);
    }
    private void CanMoveAgain()
    {
        m_CanMove = true;
    }

    public override void GetDamage(int howMuchDamage)
    {
        base.GetDamage(howMuchDamage);
        m_Animator.SetTrigger("Damaged");
        //healthBar.UpdateHealthBar(m_MaxLifePoints, m_CurrentLifePoints);
        // If helath is less than or equal to 0, destroy the skeleton
        if (m_CurrentLifePoints <= 0)
        {
            m_CanMove = false;
            DestroyGhost();
        }
    }

    public void DestroyGhost()
    {
        m_CanMove = false;
        m_Collider.enabled = false;
        m_Animator.SetTrigger("Die");
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
        Destroy(gameObject, 1);
    }
}
