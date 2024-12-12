using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    public float healthAmount = 100f;
    public float maxHealth = 100f;
    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        if(healthAmount <= 0)
        {
            Destroy(gameObject);
        }
    }

}
