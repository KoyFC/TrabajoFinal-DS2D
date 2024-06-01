using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardScript : EnemyScript
{
    private CapsuleCollider2D m_Collider;
    private Rigidbody2D m_Rigidbody2D;
    public Transform m_InitialSummonPoint;
    public Transform[] m_TeleportPoints;
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    public GameObject m_ProyectilePrefab;
    

    public enum WIZARD_BEHAVIOUR
    {
        IDLE,
        BATTLE
    }
    public WIZARD_BEHAVIOUR m_WizardBehaviour = WIZARD_BEHAVIOUR.IDLE;

    // Variables
    public float m_WizardMaxSpeed = 2;
    public float m_WizardCurrentSpeed;
    public float m_StunnedTime = 0.5f;
    private bool m_CanMove;
    private int m_CurrentTeleportPointIndex = 0;
    public int m_TimesUntilTeleport = 3;
    public int m_TimesItGotHit = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentLifePoints = m_MaxLifePoints;
        m_WizardCurrentSpeed = m_WizardMaxSpeed;
        m_GoingRight = true;
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Collider = GetComponent<CapsuleCollider2D>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_CanMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        if (m_Player.GetComponent<PlayerController>().m_ActivateBossFight)
        {
            InvokeRepeating("Attack", 3, 2);
            m_WizardBehaviour = WIZARD_BEHAVIOUR.BATTLE;
            m_Player.GetComponent<PlayerController>().m_ActivateBossFight = false;
        }

        if (m_CurrentLifePoints <= 0)
        {
            if (m_CanMove)
            {
                DestroyBoss();
            }
            m_CanMove = false;
            m_Collider.enabled = false;
            m_Animator.SetTrigger("Die");
        }

        CheckIfFlipNeeded();
    }

    private void CheckIfFlipNeeded()
    {
        if (m_Player.transform.position.x < transform.position.x && m_GoingRight)
        {
            GoingRight = false;
        }
        else if (m_Player.transform.position.x > transform.position.x && !m_GoingRight)
        {
            GoingRight = true;
        }
    }

    public override void GetDamage(int howMuchDamage)
    {
        base.GetDamage(howMuchDamage);
        m_CanMove = false;
        m_Animator.SetTrigger("Damaged");
        StartCoroutine(StunnedAfterHit());

        m_TimesItGotHit++;
        if (m_TimesItGotHit >= m_TimesUntilTeleport)
        {
            m_Animator.SetTrigger("Teleport");
            m_TimesItGotHit = 0;
        }
    }

    public void Teleport()
    {
        transform.position = m_TeleportPoints[m_CurrentTeleportPointIndex].position;
        m_CurrentTeleportPointIndex++;
        if (m_CurrentTeleportPointIndex >= m_TeleportPoints.Length)
        {
            m_CurrentTeleportPointIndex = 0;
        }
        Invoke("CheckIfFlipNeeded", 0.2f);
    }

    private void Attack()
    {
        if (m_WizardBehaviour == WIZARD_BEHAVIOUR.IDLE) return;

        m_Animator.SetTrigger("Attack");
        m_CanMove = false;
        StartCoroutine(EnableMovement());
    }
    
    public void SummonProyectile()
    {
        Instantiate(m_ProyectilePrefab, m_InitialSummonPoint.position, Quaternion.identity);
    }

    private IEnumerator EnableMovement()
    {
        yield return new WaitForSeconds(0.8f);
        m_CanMove = true;
    }

    public void DestroyBoss()
    {
        Destroy(gameObject, 2);
    }

    private IEnumerator StunnedAfterHit()
    {
        yield return new WaitForSeconds(m_StunnedTime);
        m_CanMove = true;
        m_SpriteRenderer.color = Color.white;
        m_Animator.ResetTrigger("Damaged");
    }
}
