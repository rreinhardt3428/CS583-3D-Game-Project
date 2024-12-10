using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Player transform reference
    public float detectionRange = 10f; // Range at which enemy starts chasing the player
    public float attackRange = 5f; // Range at which enemy starts firing
    public float moveSpeed = 3f; // Walking speed of the enemy
    public float chaseSpeed = 5f; // Speed when chasing the player
    public float fireRate = 1f; // Time between shots
    public Transform gunTransform; // The transform of the enemy's gun (where the shot is fired from)
    public ParticleSystem muzzleFlash; // Muzzle flash effect
    public GameObject impactEffect; // Impact effect for hits
    public float gravityMultiplier = 2f; // Controls how fast gravity affects the enemy
    public float minDistanceFromPlayer = 2f;

    private float fireCooldown = 0f; // Time before the enemy can shoot again
    private bool isChasing = false; // Whether the enemy is chasing the player
    private CharacterController characterController; // Reference to the CharacterController for movement
    private Vector3 velocity; // Used for gravity and movement
    private bool isGrounded; // Check if the enemy is grounded

    private void Start()
    {
        characterController = GetComponent<CharacterController>(); // Get the CharacterController component
    }

    private void Update()
    {
        // Check if the enemy is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

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
        if (!isGrounded)
        {
            velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime; // Apply gravity
        }
        else
        {
            velocity.y = -0.5f; // Small value to ensure it stays grounded
        }

        // Apply the velocity to move the enemy
        characterController.Move(velocity * Time.deltaTime);
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        // Check if the enemy is too close to the player
        if (distanceToPlayer > minDistanceFromPlayer)
        {
            // Move towards the player at chase speed
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 movement = direction * chaseSpeed;
            movement.y = velocity.y;  // Maintain current y velocity for gravity

            characterController.Move(movement * Time.deltaTime);

            // Optionally, rotate the enemy to face the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void WalkAround()
    {
        // Implement walking behavior (patrol, random walk, etc.)
        // This is a placeholder, replace it with your actual walking logic
        Vector3 moveDirection = transform.forward * moveSpeed;
        moveDirection.y = velocity.y; // Preserve current y velocity for gravity
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void FireAtPlayer()
    {
        if (fireCooldown <= 0f)
        {
            // Perform hitscan shooting from enemy's gun
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
        Ray ray = new Ray(gunTransform.position, (player.position - gunTransform.position).normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
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
    }
}
