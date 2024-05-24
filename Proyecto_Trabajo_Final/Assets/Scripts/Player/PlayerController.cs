using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Spawn variables
    public Transform m_SpawnPoint;

    [Header("Player variables")]
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_Movement;

    public float m_DefaultSpeed = 5.0f;
    private float m_CurrentSpeed;
    public float m_RunSpeedMultiplier = 1.5f;

    public float m_DefaultJumpForce = 800.0f;
    private float m_CurrentJumpForce;
    private int m_RemainingExtraJumps;
    public int m_DefaultMaxExtraJumps = 1;
    private int m_CurrentMaxExtraJumps;

    public float m_KnockbackForce;
    private bool m_CanMove;
    private bool m_IsDead;
    public bool m_ReviveTriggered;

    private Vector3 m_MousePosition;
    private Animator m_Animator;
    private SpriteRenderer m_PlayerRenderer;
    public GameObject m_JumpParticlesPrefab;
    public GameObject m_JumpParticlesSpawn;

    [Header("Life and UI")]
    public int m_MaxLifePoints = 5;
    public int m_LifePoints;
    private bool m_InvencibleAfterHit;
    public float m_InvencibleAfterHitDuration;
    private float m_RemainingInvencibleAfterHitDuration;
    private bool m_NoControlAfterHit;
    private float m_NoControlAfterHitDuration;
    private float m_RemainingNoControlAfterHitDuration;

    [Header("Ground check variables")]
    public Transform m_GroundCheck;
    public LayerMask m_GroundLayer;
    private bool m_IsGrounded;

    [Header("Input variables")]
    private bool m_RunPressed;
    private bool m_JumpPressed;
    private bool m_SitPressed;
    private bool m_SummonLanternPressed;
    private bool m_AlternateColorPressed;
    private bool m_LeftClickPressed;
    private bool m_RightClickPressed;

    [Header("Lantern variables")]
    public GameObject m_Lantern;
    public Transform m_LanternHinge; // The hinge that the lantern will rotate around
    public Renderer m_LanternRenderer0; // The lantern's main renderer
    public Renderer m_LanternRenderer1; // The lantern's light is divided into two shapes, thus 2 renderers
    public Renderer m_LanternRenderer2;
    private PolygonCollider2D m_LightCollider;
    private LightDamageScript m_LightDamageScript;
    public Color[] m_LanternColors; // Set of colors that the lantern can have
    public float m_DefaultActionCooldown = 1.5f;
    private float m_CurrentActionCooldown;
    public bool m_CanPerformLanternAction;

    private int m_CurrentColorIndex; // Index of the current color in the array
    public int m_UnlockedColors; // Number of colors that the player has unlocked
    private bool m_LanternActive; 

    // Direction variables
    private bool m_GoingRight;
    public bool GoingRight
    {
        get { return m_GoingRight; }
        set 
        { 
            // Flip player and children
            if (m_GoingRight != value)
            {
                transform.localScale = new Vector2
                    (transform.localScale.x * -1, 
                    transform.localScale.y);
            }
            m_GoingRight = value; 
        }
    }

    void Start()
    {
        m_LifePoints = m_MaxLifePoints;
        m_CanMove = true;
        m_NoControlAfterHit = false;
        m_IsDead = false;
        m_GoingRight = true;
        m_RemainingExtraJumps = m_DefaultMaxExtraJumps;
        m_CanPerformLanternAction = true;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_PlayerRenderer = GetComponent<SpriteRenderer>();
        m_LightDamageScript = m_Lantern.GetComponentInChildren<LightDamageScript>();
        m_LightCollider = m_Lantern.GetComponentInChildren<PolygonCollider2D>();
        //m_UnlockedColors = 1; // The player will always start with the default color unlocked
        m_RemainingInvencibleAfterHitDuration = m_InvencibleAfterHitDuration;
        m_NoControlAfterHitDuration = m_InvencibleAfterHitDuration * 3/4;
    }

    void Update()
    {
        if (!m_IsDead)
        {
            HandleInputs();
            HandleMovement();
            HandleJump();
            HandleAnimations();
            HandleLife();
            HandleInvincibility();
            HandlePlayerBenefits();

            if (m_LanternActive)
            {
                AimLantern();
            }

            if (m_CanPerformLanternAction)
            {
                SwitchPlayerColor();
                SwitchLanternColor();
            }
        }
    }

    // --- PLAYER METHODS ---

    private void HandleInputs()
    {
        if (!m_NoControlAfterHit)
        {
            m_Movement.x = Input.GetAxis("Horizontal");
            m_Movement.y = Input.GetAxisRaw("Vertical");
            m_RunPressed = Input.GetKey(KeyCode.LeftShift);
            m_JumpPressed = Input.GetKeyDown(KeyCode.Space);
            m_SitPressed = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.LeftControl);
            m_SummonLanternPressed = Input.GetKeyDown(KeyCode.E);
            m_AlternateColorPressed = Input.GetKeyDown(KeyCode.Q);
            m_LeftClickPressed = Input.GetMouseButtonDown(0);
            m_RightClickPressed = Input.GetMouseButtonDown(1);
        }
    }

    public void ToggleCanMove()
    {
        m_CanMove = !m_CanMove;
    }

    private void HandleMovement()
    {
        if (m_CanMove)
        {
            m_IsGrounded = Physics2D.OverlapBox(
                m_GroundCheck.position, 
                new Vector2(0.8f, 0.35f), 
                0, 
                m_GroundLayer); // Create a temporal square box to check if the player is grounded
        }
        if (m_NoControlAfterHit && m_CanPerformLanternAction)
        {
            m_Animator.SetTrigger("ForceIdle");
            return;
        }
        
        // Determine the speed multiplier based on whether the player is running or not
        float speedMultiplier;
        if (m_RunPressed && !m_LanternActive)
        {
            speedMultiplier = m_CurrentSpeed * m_RunSpeedMultiplier;
        }
        else
        {
            speedMultiplier = m_CurrentSpeed;
        }

        // Move the player by setting the velocity of the Rigidbody only if it is able to move

        if (m_CanMove)
        {
            Vector2 resultingVelocity = new Vector2(
                m_Movement.x * speedMultiplier * m_CurrentSpeed, 
                m_Rigidbody2D.velocity.y);
            
            m_Rigidbody2D.velocity = resultingVelocity;
        }
        else // If the player can't move, the velocity is set to 0
        {
            m_Rigidbody2D.velocity = Vector2.zero;
        }
        
        // Flip the player's sprite based on the mouse position only if the player is not running
        m_MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_MousePosition.z = transform.position.z;

        if (m_LanternActive) // The player will flip based on the mouse position only if the lantern is active
        {
            if (m_MousePosition.x > transform.position.x)
            {
                GoingRight = true;
            }
            else
            {
                GoingRight = false;
            }
        }

        else // The player will flip based on the movement direction if the lantern is not active
        {
            if (m_Movement.x > 0)
            {
                GoingRight = true;
            }
            else if (m_Movement.x < 0)
            {
                GoingRight = false;
            }
        }
    }

    private void HandleJump()
    {
        // Handle jump
        if (m_IsGrounded)
        {
            m_RemainingExtraJumps = m_CurrentMaxExtraJumps;
        }

        // We handle the animation here since it is related to the jump and would require extra checks in the HandleAnimations method
        if (m_IsGrounded && m_JumpPressed && m_CanMove)
        {
            m_Rigidbody2D.AddForce(Vector2.up * m_CurrentJumpForce);
            m_Animator.SetTrigger("JumpPressed");
        }
        else if (!m_IsGrounded && m_JumpPressed && m_CanMove && m_RemainingExtraJumps > 0)
        {
            // Set vertical velocity to 0 and then add the jump force
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            m_Rigidbody2D.AddForce(Vector2.up * m_CurrentJumpForce);
            m_RemainingExtraJumps--;
            SummonAirJumpParticles();
            m_Animator.SetTrigger("JumpPressed");
        }
    }

    private void SummonAirJumpParticles()
    {
        // Instantiate the jump particles prefab rotated 90 degrees
        Instantiate(m_JumpParticlesPrefab, m_JumpParticlesSpawn.transform.position, Quaternion.Euler(0, 0, 90));        
    }

    private void HandleAnimations()
    {
        if (m_Rigidbody2D.velocity.x == 0 && m_Movement.x == 0)
        {
            m_Animator.SetBool("IsWalking", false);
            m_Animator.SetBool("IsRunning", false);
        }
        else if (m_Rigidbody2D.velocity.x != 0 && !m_RunPressed || m_Rigidbody2D.velocity.x != 0 && m_RunPressed && m_LanternActive)
        {
            m_Animator.SetBool("IsWalking", true);
            m_Animator.SetBool("IsRunning", false);
        }
        else if (m_Rigidbody2D.velocity.x != 0 && m_RunPressed && !m_LanternActive)
        { 
            m_Animator.SetBool("IsWalking", false);
            m_Animator.SetBool("IsRunning", true);
        }
        if (m_SitPressed && m_IsGrounded)
        {
            m_Animator.SetTrigger("SitPressed");
        }

        if (m_SummonLanternPressed)
        {
            m_Animator.SetTrigger("ActiveLantern");
        }

        if (m_LeftClickPressed && m_CanPerformLanternAction && m_LanternActive)
        {
            m_Animator.SetTrigger("LanternAction");  
        }
        else if (m_LeftClickPressed && m_CanPerformLanternAction && !m_LanternActive)
        {
            if (m_PlayerRenderer.material.color != m_LanternColors[0] && m_PlayerRenderer.material.color != m_LanternColors[1])
            {
                m_Animator.SetTrigger("LanternAction");
            }
        }
    }

    public void SwitchPlayerColor()
    {
        if (m_LanternActive)
        {
            m_PlayerRenderer.material.color = m_LanternRenderer0.material.color;
        }
        else if (!m_LanternActive && m_SitPressed)
        {
            m_PlayerRenderer.material.color = m_LanternColors[0];
        }
    }

    private void HandlePlayerBenefits()
    {
        if (m_PlayerRenderer.material.color == m_LanternColors[1]) // Red
        {
            m_CurrentSpeed = m_DefaultSpeed * 0.85f;
            m_CurrentJumpForce = m_DefaultJumpForce * 0.95f;
            m_CurrentMaxExtraJumps = m_DefaultMaxExtraJumps;
        }
        else if (m_PlayerRenderer.material.color == m_LanternColors[2]) // Blue
        {
            m_CurrentSpeed = m_DefaultSpeed * 0.925f;
            m_CurrentJumpForce = m_DefaultJumpForce;
            m_CurrentMaxExtraJumps = m_DefaultMaxExtraJumps;
        }
        else if (m_PlayerRenderer.material.color == m_LanternColors[3]) // Green
        {
            m_CurrentSpeed = m_DefaultSpeed * 0.9f;
            m_CurrentJumpForce = m_DefaultJumpForce * 1.1f;
            m_CurrentMaxExtraJumps = m_DefaultMaxExtraJumps + 1;
        }
        else if (m_PlayerRenderer.material.color == m_LanternColors[4]) // Yellow
        {
            m_CurrentSpeed = m_DefaultSpeed * 1.08f;
            m_CurrentJumpForce = m_DefaultJumpForce * 0.92f;
            m_CurrentMaxExtraJumps = m_DefaultMaxExtraJumps;
        }
        else 
        {
            m_CurrentSpeed = m_DefaultSpeed;
            m_CurrentJumpForce = m_DefaultJumpForce;
            m_CurrentMaxExtraJumps = m_DefaultMaxExtraJumps;
        }
    }

    private void HandleLife()
    {
        if (m_LifePoints <= 0 && !m_IsDead)
        {
            m_IsDead = true;
            m_CanMove = false;
            m_Animator.SetTrigger("Die");
        }
    }
    
    public void ReceiveDamage(int damage, float enemyXPos)
    {
        if (!m_InvencibleAfterHit && !m_IsDead)
        {
            m_LifePoints -= damage;
            m_InvencibleAfterHit = true;
            m_NoControlAfterHit = true;

            if (enemyXPos < transform.position.x)
            {
                m_Rigidbody2D.AddForce((Vector2.right + Vector2.up) * m_KnockbackForce);
            }
            else
            {
                m_Rigidbody2D.AddForce((Vector2.left + Vector2.up) * m_KnockbackForce);
            }
        }
    }

    private void HandleInvincibility()
    {
        if (m_NoControlAfterHit)
        {
            if (m_RemainingNoControlAfterHitDuration > Mathf.Epsilon) // This is basically > 0
            {
                m_RemainingNoControlAfterHitDuration -= Time.deltaTime;
            }
            else
            {
                m_RemainingNoControlAfterHitDuration = m_NoControlAfterHitDuration;
                m_NoControlAfterHit = false;
            }
        }

        if (m_InvencibleAfterHit)
        {
            if (m_RemainingInvencibleAfterHitDuration > Mathf.Epsilon) // This is basically > 0
            {
                m_RemainingInvencibleAfterHitDuration -= Time.deltaTime;
            }
            else
            {
                m_RemainingInvencibleAfterHitDuration = m_InvencibleAfterHitDuration;
                m_InvencibleAfterHit = false;
            }
        }
    }
    
    public void TriggerRevival() // Called during death animation
    {
        m_InvencibleAfterHit = false;
        m_LanternActive = false;
        m_Lantern.SetActive(false);
        m_Animator.SetTrigger("Revive");
        m_Rigidbody2D.velocity = Vector2.zero;
        transform.position = m_SpawnPoint.position;
        GoingRight = true;
    }

    public void StartRevival() // Called during revival animation REMOVE
    {
        m_LifePoints = m_MaxLifePoints;
        m_IsDead = false;
        m_CanMove = true;
    }

    public void RespawnEnemies() // Also called during revival animation REMOVE
    {
        m_ReviveTriggered = true;
        Invoke("EndRevival", 0.3f);
    }

    private void EndRevival() // REMOVE
    {
        m_ReviveTriggered = false;
    }


    // --- LANTERN METHODS ---

    public void SummonLantern() // Used in the animation event
    {
        if (!m_LanternActive)
        {
            m_LanternActive = true;
            m_Lantern.SetActive(true);
        }
        else
        {
            m_LanternActive = false;
            m_Lantern.SetActive(false);
        }
    }

    private void AimLantern()
    {
        Vector2 direction = m_MousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjusting the lantern angle to point in the right direction
        if (!GoingRight)
        {
            // If the player is not going right, the angle is inverted
            angle += (angle < 0) ? 180 : -180;
        }
        m_LanternHinge.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void SwitchLanternColor()
    {
        if (m_RightClickPressed) // If the right mouse button is pressed, the color will cycle through the unlocked colors (excluding default)
        {
            SelectNextColor();
            m_LanternRenderer0.material.color = m_LanternColors[m_CurrentColorIndex];
            m_LanternRenderer1.material.color = m_LanternColors[m_CurrentColorIndex];
            m_LanternRenderer2.material.color = m_LanternColors[m_CurrentColorIndex];
        }

        if (m_AlternateColorPressed) // If the left mouse button is pressed, the color will be set to the default color
        {
            if (m_LanternRenderer0.material.color != m_LanternColors[0])
            {
                m_PlayerRenderer.material.color = m_LanternColors[0];
                m_LanternRenderer0.material.color = m_LanternColors[0];
                m_LanternRenderer1.material.color = m_LanternColors[0];
                m_LanternRenderer2.material.color = m_LanternColors[0];
            }
            else 
            {
                m_PlayerRenderer.material.color = m_LanternColors[m_CurrentColorIndex];
                m_LanternRenderer0.material.color = m_LanternColors[m_CurrentColorIndex];
                m_LanternRenderer1.material.color = m_LanternColors[m_CurrentColorIndex];
                m_LanternRenderer2.material.color = m_LanternColors[m_CurrentColorIndex];
            }
        }
    }

    private void SelectNextColor() // Select the next color in the array checking how many colors the player has unlocked.  
    {
        // Extra checks to avoid out of bounds errors

        if (m_UnlockedColors >= m_LanternColors.Length) 
        {
            m_UnlockedColors = m_LanternColors.Length;
        }
        else if (m_UnlockedColors < 1)
        {
            m_UnlockedColors = 1;
        }

        if (m_UnlockedColors > 1) // If the number of unlocked colors is 1 (just the default), the color will not change.
        {
            m_CurrentColorIndex++;
            if (m_CurrentColorIndex >= m_UnlockedColors)
            {
                m_CurrentColorIndex = 1; // The color will never loop back to 0 since it is reserved for the default color, accesed with another key.
            }
        }
    }

    public void LanternSpecialAction()
    {
        if (!m_CanPerformLanternAction)
        {
            return;
        }
        m_CanPerformLanternAction = false;
        m_Animator.ResetTrigger("LanternAction"); // Without this line, when spamming click, the animation will play twice

        if (m_PlayerRenderer.material.color == m_LanternColors[0]) // White
        {
            // Attack
            if (m_LanternActive)
            {
                m_LightDamageScript.m_CurrentLightDamage = m_LightDamageScript.m_DefaultLightDamage;
                m_LightCollider.enabled = true;
                StartCoroutine(DeactivateLanternCollider());
            }
            m_CurrentActionCooldown = m_DefaultActionCooldown;
        }
        if (m_PlayerRenderer.material.color == m_LanternColors[1]) // Red
        {
            if (m_LanternActive)
            {
                m_LightDamageScript.m_CurrentLightDamage = m_LightDamageScript.m_DefaultLightDamage * 2;
                m_LightCollider.enabled = true;
                m_CurrentActionCooldown = m_DefaultActionCooldown * 1.5f;
                StartCoroutine(DeactivateLanternCollider());
            }
        }
        else if (m_PlayerRenderer.material.color == m_LanternColors[2]) // Blue
        {
            // Invincible for 0.8 seconds, but can't move during most of it (the player is still allowed to move a bit before the invincibility ends)
            m_LightDamageScript.m_CurrentLightDamage = 0;
            m_InvencibleAfterHit = true;
            m_NoControlAfterHit = true;
            m_RemainingInvencibleAfterHitDuration = m_DefaultActionCooldown * 0.8f;
            m_CurrentActionCooldown = m_DefaultActionCooldown;
        }
        else if (m_PlayerRenderer.material.color == m_LanternColors[3]) // Green
        {
            // Jump that doesn't consume extra jumps
            m_LightDamageScript.m_CurrentLightDamage = 0;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            m_Rigidbody2D.AddForce(Vector2.up * m_CurrentJumpForce * 1.35f);

            if (!m_IsGrounded)
            {
                SummonAirJumpParticles();
            }

            m_Animator.SetTrigger("JumpPressed");
            m_CurrentActionCooldown = m_DefaultActionCooldown;
        }
        else if (m_PlayerRenderer.material.color == m_LanternColors[4]) // Yellow
        {
            m_LightDamageScript.m_CurrentLightDamage = 0;
            // Invert the player's gravity and its sprite vertically
            m_Rigidbody2D.gravityScale *= -1;
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1);
            m_DefaultJumpForce *= -1;
            m_CurrentActionCooldown = 0.8f;
        }

        StartCoroutine(LanternCooldown());
    }

    private IEnumerator LanternCooldown()
    {
        yield return new WaitForSeconds(m_CurrentActionCooldown);
        m_CanPerformLanternAction = true;
    }

    private IEnumerator DeactivateLanternCollider()
    {
        yield return new WaitForSeconds(0.1f);
        m_LightCollider.enabled = false;
    }


    // --- COLLISION METHODS ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyScript thisEnemy = collision.gameObject.GetComponent<EnemyScript>();
            m_Rigidbody2D.velocity = Vector2.zero;
            ReceiveDamage(thisEnemy.m_DamageDealtToPlayer, collision.transform.position.x);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathBox"))
        {
            m_LifePoints = 0;
        }
    }
}
