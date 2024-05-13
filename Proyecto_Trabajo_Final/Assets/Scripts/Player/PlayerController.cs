using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player variables")]
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_Movement;
    public float m_Speed = 5.0f;
    public float m_RunSpeed = 2.0f;
    public float m_JumpForce = 10.0f;
    private bool m_CanMove;
    private int m_RemainingExtraJumps;
    public int m_MaxExtraJumps = 1;

    private Vector3 m_MousePosition;
    private Animator m_Animator;
    private Renderer m_PlayerRenderer;

    [Header("Ground check variables")]
    public Transform m_GroundCheck;
    public LayerMask m_GroundLayer;
    private bool m_IsGrounded;

    [Header("Input variables")]
    private bool m_RunPressed;
    private bool m_JumpPressed;
    private bool m_SitPressed;
    private bool m_SummonLanternPressed;
    private bool m_LeftClickPressed;
    private bool m_RightClickPressed;

    [Header("Lantern variables")]
    public GameObject m_Lantern;
    public Transform m_LanternHinge; // The hinge that the lantern will rotate around
    public Renderer m_LanternRenderer1; // The lantern's light is divided into two shapes, thus 2 renderers
    public Renderer m_LanternRenderer2;
    public Color[] m_LanternColors; // Set of colors that the lantern can have

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
        m_CanMove = true;
        m_GoingRight = true;
        m_RemainingExtraJumps = m_MaxExtraJumps;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_PlayerRenderer = GetComponent<Renderer>();
        m_UnlockedColors = 1; // The player will always start with the default color unlocked
    }

    void Update()
    {
        HandleInputs();
        HandleMovement();
        HandleJump();
        HandleAnimations();

        if (m_LanternActive)
        {
            AimLantern();
        }

        SwitchLanternColor();
    }

    // --- PLAYER METHODS ---

    private void HandleInputs()
    {
        if (m_CanMove)
        {
            m_Movement.x = Input.GetAxis("Horizontal");
            m_Movement.y = Input.GetAxisRaw("Vertical");
            m_RunPressed = Input.GetKey(KeyCode.LeftShift);
            m_JumpPressed = Input.GetKeyDown(KeyCode.Space);
            m_SitPressed = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.LeftControl);
            m_SummonLanternPressed = Input.GetKeyDown(KeyCode.E);
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
        
        // Determine the speed multiplier based on whether the player is running or not
        float speedMultiplier;
        if (m_RunPressed && !m_LanternActive)
        {
            speedMultiplier = m_Speed * m_RunSpeed;
        }
        else
        {
            speedMultiplier = m_Speed;
        }

        // Move the player by setting the velocity of the Rigidbody only if it is able to move

        if (m_CanMove)
        {
            Vector2 resultingVelocity = new Vector2(
                m_Movement.x * speedMultiplier * m_Speed, 
                m_Rigidbody2D.velocity.y);
            
            m_Rigidbody2D.velocity = resultingVelocity;
        }
        else // If the player is not able to move, the velocity is set to zero
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
            m_RemainingExtraJumps = m_MaxExtraJumps;
        }

        // We handle the animation here since it is related to the jump and would require extra checks in the HandleAnimations method
        if (m_IsGrounded && m_JumpPressed && m_CanMove)
        {
            m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce);
            m_Animator.SetTrigger("JumpPressed");
        }
        else if (!m_IsGrounded && m_JumpPressed && m_CanMove && m_RemainingExtraJumps > 0)
        {
            // Set vertical velocity to 0 and then add the jump force
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce);
            m_RemainingExtraJumps--;
            m_Animator.SetTrigger("JumpPressed");
        }
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
    }

    public void SwitchPlayerColor()
    {
        if (m_LanternActive)
        {
            m_PlayerRenderer.material.color = m_LanternRenderer1.material.color;
        }
        else 
        {
            m_PlayerRenderer.material.color = m_LanternColors[0];
        }
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
            m_LanternRenderer1.material.color = m_LanternColors[m_CurrentColorIndex];
            m_LanternRenderer2.material.color = m_LanternColors[m_CurrentColorIndex];
        }

        if (m_LeftClickPressed) // If the left mouse button is pressed, the color will be set to the default color
        {
            m_LanternRenderer1.material.color = m_LanternColors[0];
            m_LanternRenderer2.material.color = m_LanternColors[0];
        }
    }

    private void SelectNextColor() // Select the next color in the array checking how many colors the player has unlocked.  
    {
        if (m_UnlockedColors >= m_LanternColors.Length) // Extra check to avoid out of bounds errors
        {
            m_UnlockedColors = m_LanternColors.Length;
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
}
