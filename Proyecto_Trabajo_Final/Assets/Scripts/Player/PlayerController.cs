using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement variables
    private Vector2 m_Movement;
    public float m_Speed = 5.0f;
    public float m_RunSpeed = 2.0f;

    // Animation variables
    private Animator m_Animator;

    // Input variables
    public bool m_Run;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
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
        float speedMultiplier;
        // Move the player
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        
        if (m_Run)
        {
            speedMultiplier = m_Speed * m_RunSpeed;
        }
        else
        {
            speedMultiplier = m_Speed;
        }

        Vector2 newPosition = currentPosition + speedMultiplier * Time.deltaTime * m_Movement;
        transform.position = newPosition;

        // Flip the player's sprite based on the mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePosition.z = transform.position.z;

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
        if (m_Movement.x != 0 && !m_Run)
        {
            m_Animator.SetBool("IsWalking", true);
        }
        else if (m_Movement.x == 0)
        {
            m_Animator.SetBool("IsWalking", false);
        }
        
        if (m_Movement.x != 0 && m_Run)
        {
            m_Animator.SetBool("IsRunning", true);
        }
        else if (m_Movement.x == 0)
        {
            m_Animator.SetBool("IsRunning", false);
        }
    }
}
