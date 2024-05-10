using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement variables
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_Movement;
    public float m_Speed = 5.0f;
    public float m_RunSpeed = 2.0f;
    private bool m_CanMove;

    // Animation variables
    private Animator m_Animator;

    // Input variables
    private bool m_RunPressed;
    private bool m_JumpPressed;
    private bool m_SummonLanternPressed;
    private bool m_LeftClickPressed;
    private bool m_RightClickPressed;

    // Lantern variables
    public GameObject m_Lantern;
    public Transform m_LanternHinge;
    private Vector3 m_MousePosition;
    private bool m_LanternActive;
    public Color m_DefaultColor;
    public Color m_Color1;
    public Color m_Color2;
    public Renderer m_LanternRenderer1;
    public Renderer m_LanternRenderer2;
    private bool m_LanternColorSwitch;

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
        m_Animator = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInputs();
        HandleMovement();
        HandleAnimations();

        if (m_LanternActive)
        {
            AimLantern();
        }

        SwitchColor();
    }

    private void HandleInputs()
    {
        if (m_CanMove)
        {
            m_Movement.x = Input.GetAxis("Horizontal");
            m_Movement.y = Input.GetAxis("Vertical");
            m_RunPressed = Input.GetKey(KeyCode.LeftShift);
            m_JumpPressed = Input.GetKeyDown(KeyCode.Space);
            m_SummonLanternPressed = Input.GetKeyDown(KeyCode.E);
            m_LeftClickPressed = Input.GetMouseButtonDown(0);
            m_RightClickPressed = Input.GetMouseButtonDown(1);
        }
    }

    // --- PLAYER METHODS ---

    public void ToggleCanMove()
    {
        m_CanMove = !m_CanMove;
    }

    private void HandleMovement()
    {
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
        
        if (m_JumpPressed)
        {
            m_Animator.SetTrigger("JumpPressed");
        }

        if (m_SummonLanternPressed)
        {
            m_Animator.SetTrigger("ActiveLantern");
        }
    }

    // --- LANTERN METHODS ---

    public void SummonLantern()
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

    void SwitchColor() // TODO: 
    {
        // Change the color of both renderers to the same color and back
        if (m_RightClickPressed)
        {
            // For the program to remember the last used color
            if (m_LanternRenderer1.material.color == m_Color1) 
            {
                m_LanternColorSwitch = true;
            }
            else if (m_LanternRenderer1.material.color == m_Color2)
            {
                m_LanternColorSwitch = false;
            } 

            // Switch the color of the lanterns
            if (m_LanternColorSwitch)
            {
                m_LanternRenderer1.material.color = m_Color2;
                m_LanternRenderer2.material.color = m_Color2;
            }
            else
            {
                m_LanternRenderer1.material.color = m_Color1;
                m_LanternRenderer2.material.color = m_Color1;
            } 
        }

        // Change the color of both renderers to the default color
        if (m_LeftClickPressed)
        {
            m_LanternRenderer1.material.color = m_DefaultColor;
            m_LanternRenderer2.material.color = m_DefaultColor;
        }      
    }
}
