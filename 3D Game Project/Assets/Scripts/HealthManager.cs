using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100f;
    public float maxHealth = 100f;
    private Coroutine autoHealCoroutine;

    // Update is called once per frame
    void Update()
    {
        // Testing the health function
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(10);
        }
        
        /* // Will have passive regen 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal(5);
        } */
        
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;

        if(autoHealCoroutine != null)
        {
            StopCoroutine(autoHealCoroutine);
        }
        autoHealCoroutine = StartCoroutine(AutoHeal());
    }

    IEnumerator AutoHeal()
    {
        yield return new WaitForSeconds(5f);

        while (healthAmount < maxHealth)
        {
            Heal(.2f);
            yield return new WaitForSeconds(.02f);
        }
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        healthBar.fillAmount = healthAmount / 100f;
    }
}