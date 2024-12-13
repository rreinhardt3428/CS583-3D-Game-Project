using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun1Shooting : MonoBehaviour
{
    public float Cliplength = 1f;
    public AudioClip ShootSound;
    public AudioClip ReloadSound;  // Added reload sound variable
    private AudioSource audioSource;

    public Cinemachine.CinemachineVirtualCamera thirdPersonCamera;
    public GameObject bulletEffectPrefab;
    public Transform gunPoint;
    public float bulletSpeed = 500f;
    public float fireRate = 0.15f;
    public float range = 1000f; 
    public int damage = 10; // can be changed for each gun in inspector 
    private float timer = 0f;
    public int maxClip = 30;
    public int clip;
    public int totalAmmo;
    public float reloadTime = 1f;
    public bool reloading = false;
    public bool unlimitedAmmo = false;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public TMP_Text ammoText;

    void Start()
    {
        clip = maxClip;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found, adding one.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set default sound clips
        audioSource.playOnAwake = false;
        totalAmmo = 300;
        clip = maxClip;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (reloading) return;

        if (Input.GetButton("Fire1") && Time.time >= timer)
        {
            if (clip > 0)
            {
                timer = Time.time + fireRate;
                Shoot();
            }
            else
            {
                //Debug.Log("Out of ammo.");
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && clip < maxClip)
        {
            StartCoroutine(Reload());
        }

    }

    void Shoot()
    {
        gunPoint.rotation = thirdPersonCamera.VirtualCameraGameObject.transform.rotation;
        if (!unlimitedAmmo)
        {
            clip--;
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        Transform cameraTransform = thirdPersonCamera.VirtualCameraGameObject.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        Vector3 targetPoint = cameraTransform.position + cameraTransform.forward * range;
        if (Physics.Raycast(ray, out hit, range))
        {
            targetPoint = hit.point;

            EnemyHealthManager enemyHealth = hit.collider.GetComponent<EnemyHealthManager>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(10);
            }

            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, .5f);
            }
            Debug.Log($"Hit {hit.collider.name} at {hit.point}");
        }

        else
        {
            //Debug.Log("Missed");
        }
    
        if (bulletEffectPrefab != null)
        {
            GameObject bullet = Instantiate(bulletEffectPrefab, gunPoint.position, Quaternion.identity);
            Vector3 direction = (targetPoint - gunPoint.position).normalized;
            StartCoroutine(MoveBullet(bullet, targetPoint, direction));
            // Destroy(bullet, 3f);
        }
        if (audioSource != null && ShootSound != null)
        {
            Debug.Log("Playing shoot sound.");
            audioSource.PlayOneShot(ShootSound); // Play the sound
        }

        UpdateAmmoUI();
    }

    IEnumerator MoveBullet(GameObject bullet, Vector3 targetPoint, Vector3 direction)
    {
        float distance = Vector3.Distance(gunPoint.position, targetPoint);
        float travelTime = distance / bulletSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            if (bullet == null)
            {
                yield break;
            }
            bullet.transform.position += direction * bulletSpeed * Time.deltaTime;
            if (Vector3.Distance(bullet.transform.position, targetPoint) < .1f)
            {
                Destroy(bullet);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (bullet != null)
        {
            Destroy(bullet);
        }

    }

    IEnumerator Reload()
    {
        reloading = true;
        Debug.Log("Reloading...");

        if (audioSource != null && ReloadSound != null)
        {
            audioSource.PlayOneShot(ReloadSound);
        }

        yield return new WaitForSeconds(reloadTime);
        int ammoToReload = maxClip - clip;
        clip += ammoToReload;
        totalAmmo -= ammoToReload;
        reloading = false;
        Debug.Log("Reloaded.");
        UpdateAmmoUI();
    }

    void UpdateAmmoUI() {
        ammoText.text = clip + "/" + totalAmmo;
    }
}
