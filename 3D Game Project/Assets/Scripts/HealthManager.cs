using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100f;
    public float maxHealth = 100f;
    public Transform respawnPoint;
    public float respawnDelay = 2f;
    private bool isDead = false;
    private Coroutine autoHealCoroutine;
    private PlayerMovement playerMovement;
    private Gun1Shooting shootingScript;
    public DeathCountdown deathCountdown;

    
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        shootingScript = GetComponentInParent<Gun1Shooting>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;

        if(healthAmount <= 0)
        {
            Die();
        }

        if(autoHealCoroutine != null)
        {
            StopCoroutine(autoHealCoroutine);
        }
        if (healthAmount > 0)
        {
            autoHealCoroutine = StartCoroutine(AutoHeal());
        }
    }

    private void Die()
    {
        isDead = true;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        shootingScript.enabled = false;
        
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        deathCountdown.StartCountdown();
        

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        healthAmount = maxHealth;
        isDead = false;

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }

        if (healthBar != null)
        {
            healthBar.fillAmount = healthAmount / maxHealth;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        playerMovement.enabled = true;
        shootingScript.enabled = true;
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