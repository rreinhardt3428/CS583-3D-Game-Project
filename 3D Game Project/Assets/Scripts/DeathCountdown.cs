using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathCountdown : MonoBehaviour
{
    public TMPro.TextMeshProUGUI countdownText;
    public float respawnTime = 2f;
    private float countdown;

    void Start()
    {
        countdownText.text = "";
        
    }

    public void StartCountdown()
    {
        countdown = respawnTime;
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        while (countdown > 0)
        {
            countdownText.text = $"Respawning in {Mathf.Ceil(countdown)}...";
            countdown -= Time.deltaTime;
            yield return null;
        }
        countdownText.text = "";
    }

}
