using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDamageScript : MonoBehaviour
{
    public int m_DefaultLightDamage = 1;
    public int m_CurrentLightDamage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Intento conseguir el componente SlimeController del objeto con el que colisiono
            EnemyScript thisEnemy = other.GetComponentInParent<EnemyScript>();
            // Si no es null, significa que el object tiene el componente SlimeController;
            if (thisEnemy != null)
            {
                thisEnemy.GetDamage(m_CurrentLightDamage);
            }
        }
    }
}
