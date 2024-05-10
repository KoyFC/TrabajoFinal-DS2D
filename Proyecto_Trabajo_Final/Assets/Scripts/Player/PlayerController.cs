using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement variables
    private Vector2 m_Movement;
    public float m_Speed = 5.0f;
    public float m_RunSpeed = 2.0f;
    private Rigidbody2D m_Rigidbody2D;

    // Animation variables
    private Animator m_Animator;

    // Input variables
    public bool m_Run;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
        HandleMovement();
        HandleAnimations();
    }

    private void HandleInputs()
    {
        m_Movement.x = Input.GetAxis("Horizontal");
        m_Movement.y = Input.GetAxis("Vertical");
        m_Run = Input.GetKey(KeyCode.LeftShift);
    }

    private void HandleMovement()
    {
        // Determine the speed multiplier based on whether the player is running or not
        float speedMultiplier;
        if (m_Run)
        {
            speedMultiplier = m_Speed * m_RunSpeed;
        }
        else
        {
            speedMultiplier = m_Speed;
        }

        // Move the player by setting the velocity of the Rigidbody
        Vector2 resultingVelocity = new Vector2(
            m_Movement.x * speedMultiplier * m_Speed, 
            m_Rigidbody2D.velocity.y);
            
        m_Rigidbody2D.velocity = resultingVelocity;

        // Flip the player's sprite based on the mouse position only if the player is not running
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x > transform.position.x)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private void HandleAnimations()
    {
        Debug.Log(m_Movement.x);
        if (m_Rigidbody2D.velocity.x == 0 && m_Movement.x == 0)
        {
            m_Animator.SetBool("IsWalking", false);
            m_Animator.SetBool("IsRunning", false);
        }
        else if (m_Rigidbody2D.velocity.x != 0 && m_Run)
        {
            m_Animator.SetBool("IsRunning", true);
            m_Animator.SetBool("IsWalking", false);
        }
        else if (m_Rigidbody2D.velocity.x != 0 && !m_Run)
        {
            m_Animator.SetBool("IsWalking", true);
            m_Animator.SetBool("IsRunning", false);

        }
        
        
    }
}
