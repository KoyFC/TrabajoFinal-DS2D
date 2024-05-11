using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform m_Target;
    public float m_HorizontalOffset;
    public float m_HorizontalSpeed;

    public Vector3 m_TargetPosition;

    private void FixedUpdate()
    {
        // Posicion de destino == posicion del player en X e Y
        m_TargetPosition = new Vector3(
            m_Target.transform.position.x, 
            m_Target.transform.position.y + 1, 
            transform.position.z);

        // Si el player mira a la derecha, añadimos offset a la derecha (lo añadimos en X)
        if(m_Target.localScale.x > 0f)
        {
            m_TargetPosition = new Vector3(
                m_TargetPosition.x + m_HorizontalOffset,
                m_TargetPosition.y,
                m_TargetPosition.z);
        }
        // Si el player mira a la izquierda, añadimos offset a la izquierda (lo quitamos en x)
        else
        {
            m_TargetPosition = new Vector3(
                m_TargetPosition.x - m_HorizontalOffset,
                m_TargetPosition.y,
                m_TargetPosition.z);
        }
        // Aplicamos la posicion en la camara
        transform.position = Vector3.Lerp(
            transform.position, 
            m_TargetPosition,
            Time.deltaTime * m_HorizontalSpeed
            );
    }
}
