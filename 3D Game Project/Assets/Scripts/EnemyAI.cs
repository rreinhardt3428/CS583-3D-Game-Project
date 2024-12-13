using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float Cliplength = 1f;
    public AudioClip ShootSound;
    public AudioClip ReloadSound;  // Added reload sound variable
    private AudioSource audioSource;

    public Transform player; // Player transform reference
    public float detectionRange = 10f; // Range at which enemy starts chasing the player
    public float attackRange = 5f; // Range at which enemy starts firing
    public float moveSpeed = 3f; // Walking speed of the enemy
    public float chaseSpeed = 5f; // Speed when chasing the player

    public float fireRate = 1f; // Time between shots
    public ParticleSystem muzzleFlash; // Muzzle flash effect
    public GameObject impactEffect; // Impact effect for hits
    public GameObject bulletEffectPrefab; //Prefab for bullet effect
    public Transform gunPoint;
    public float bulletSpeed = 500f;

    public float gravityMultiplier = 2f; // Controls how fast gravity affects the enemy
    public float minDistanceFromPlayer = 2f;

    private float fireCooldown = 0f; // Time before the enemy can shoot again
    private bool isChasing = false; // Whether the enemy is chasing the player
    private CharacterController characterController; // Reference to the CharacterController for movement
    private Vector3 velocity; // Used for gravity and movement

    private void Start()
    {
        characterController = GetComponent<CharacterController>(); // Get the CharacterController component

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found, adding one.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set default sound clips
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within detection range, start chasing
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        // Handle movement
        if (isChasing)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            WalkAround();
        }

        // Handle firing if in attack range
        if (isChasing && distanceToPlayer <= attackRange)
        {
            FireAtPlayer();
        }

        // Apply gravity to the enemy if not grounded
        if (!characterController.isGrounded)
        {
            velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime; // Apply gravity
        }
        else
        {
            velocity.y = -0.5f; // Small value to ensure it stays grounded
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        // Apply the velocity to move the enemy
        // Apply gravity manually, but normalize the horizontal velocity
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);  // Ignore the vertical velocity (Y-axis)
        horizontalVelocity = horizontalVelocity.normalized * characterController.velocity.magnitude; // Normalize and scale by the current speed

        // Apply the gravity effect to the Y component of the velocity
        velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);

        // Move the character with the normalized horizontal velocity
        characterController.Move(velocity * Time.deltaTime);
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > minDistanceFromPlayer)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 movement = direction * chaseSpeed;
            movement.y = velocity.y;  // Maintain current y velocity for gravity

            characterController.Move(movement * Time.deltaTime);
        }

        // Rotate to face the player
        RotateToFacePlayer();
    }

    private void WalkAround()
    {
        // Implement walking behavior (patrol, random walk, etc.)
        // This is a placeholder, replace it with your actual walking logic
        Vector3 moveDirection = transform.forward.normalized * moveSpeed;
        moveDirection.y = velocity.y; // Preserve current y velocity for gravity
        characterController.Move(moveDirection * Time.deltaTime);
    }

    public float GetCurrentSpeed()
    {
        return characterController.velocity.magnitude;
    }

    public bool IsFiring()
    {
        return isChasing && Vector3.Distance(transform.position, player.position) <= attackRange;
    }

    private void FireAtPlayer()
    {
        // Rotate to face the player before firing
        RotateToFacePlayer();

        if (fireCooldown <= 0f)
        {
            ShootFromGun();
            fireCooldown = fireRate;
        }
        else
        {
            fireCooldown -= Time.deltaTime;
        }
    }


    private void ShootFromGun()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Raycast to check if the shot hits the player
        Ray ray = new Ray(gunPoint.position, (player.position - gunPoint.position).normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {

            HealthManager healthManager = hit.collider.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(10);
            }

            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, .5f);
            }

            // Log for debugging (you can add damage handling here)
            Debug.Log($"Enemy hit {hit.collider.name} at {hit.point}");
        }
        else
        {
            Debug.Log("Enemy missed");
        }
        if (bulletEffectPrefab != null)
        {
            GameObject bullet = Instantiate(bulletEffectPrefab, gunPoint.position, Quaternion.identity);
            Vector3 direction = (player.position - gunPoint.position).normalized;
            StartCoroutine(MoveBullet(bullet, player.position, direction));
            // Destroy(bullet, 3f);
        }
        if (audioSource != null && ShootSound != null)
        {
            Debug.Log("Playing shoot sound.");
            audioSource.PlayOneShot(ShootSound); // Play the sound
        }
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
    private void RotateToFacePlayer()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Ignore the Y-axis to keep the enemy upright
        directionToPlayer.y = 0;

        // Only rotate if there's a meaningful direction
        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation
        }
    }
}
