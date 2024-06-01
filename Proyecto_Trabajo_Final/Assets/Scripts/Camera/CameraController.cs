using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform m_Target;
    public float m_HorizontalOffset;
    public float m_HorizontalSpeed;
    public float m_StopCamera = -5;
    public Vector2 m_BossArenaStart;
    public Vector2 m_BossArenaEnd;
    public float m_VerticalOffsetBossArena;
    public Vector2 BossArenaCameraPosition;
    public Vector3 m_TargetPosition;

    void Start()
    {
        transform.position = new Vector3(
            m_Target.transform.position.x + m_HorizontalOffset,
            m_Target.transform.position.y + 1,
            transform.position.z);
    }

    private void FixedUpdate()
    {
        if (m_Target.transform.position.x > m_BossArenaStart.x && m_Target.transform.position.x < m_BossArenaEnd.x && m_Target.transform.position.y < m_BossArenaStart.y && m_Target.transform.position.y > m_BossArenaEnd.y)
        {
            m_TargetPosition = new Vector3(
                BossArenaCameraPosition.x, 
                m_Target.transform.position.y + m_VerticalOffsetBossArena, 
                transform.position.z);

            m_TargetPosition = new Vector3(
                BossArenaCameraPosition.x,
                m_TargetPosition.y,
                m_TargetPosition.z);

            transform.position = Vector3.Lerp(
                transform.position, 
                m_TargetPosition,
                Time.deltaTime * m_HorizontalSpeed);
        }
        else
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        // Posicion de destino == posicion del player en X e Y
        m_TargetPosition = new Vector3(
            m_Target.transform.position.x, 
            m_Target.transform.position.y + 1, 
            transform.position.z);

        // Si el player mira a la derecha, a�adimos offset a la derecha (lo a�adimos en X)
        if(m_Target.localScale.x > 0f)
        {
            m_TargetPosition = new Vector3(
                m_TargetPosition.x + m_HorizontalOffset,
                m_TargetPosition.y,
                m_TargetPosition.z);
        }
        // Si el player mira a la izquierda, a�adimos offset a la izquierda (lo quitamos en x)
        else
        {
            m_TargetPosition = new Vector3(
                m_TargetPosition.x - m_HorizontalOffset,
                m_TargetPosition.y,
                m_TargetPosition.z);
        }
        // Aplicamos la posicion en la camara
        if (m_Target.transform.position.y > m_StopCamera)
        {
            transform.position = Vector3.Lerp(
                transform.position, 
                m_TargetPosition,
                Time.deltaTime * m_HorizontalSpeed);
        }
    }
}
