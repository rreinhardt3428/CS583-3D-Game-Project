using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPowerup : MonoBehaviour
{
    public float frMulti = 0.5f;
    public float duration = 5f;
    private Renderer powerupRenderer;
    private Collider powerupCollider;

    void Start()
    {
        powerupRenderer = GetComponent<Renderer>();
        powerupCollider = GetComponent<Collider>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // this specifically searching on the camera because i have the gun attached to it
            // this would need to be edited in the future for the specific model which i can do whenever
            Gun1Shooting gun = other.GetComponent<Gun1Shooting>();
            StartCoroutine(PowerUp(gun));
            powerupRenderer.enabled = false;
            powerupCollider.enabled = false;
        }
    }

    IEnumerator PowerUp(Gun1Shooting gun)
    {
        Debug.Log("Fire rate power up on");
        float currFireRate = gun.fireRate;
        bool unlimited = gun.unlimitedAmmo;

        gun.fireRate *= frMulti;
        gun.unlimitedAmmo = true;

        yield return new WaitForSeconds(duration);

        gun.fireRate = currFireRate;
        gun.unlimitedAmmo = unlimited;

        Debug.Log("Power up off.");
        Destroy(gameObject);
    }
}
