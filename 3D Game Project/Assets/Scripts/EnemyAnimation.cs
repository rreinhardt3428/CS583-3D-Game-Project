using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyAnimationBehavior : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    public AvatarMask avatarMask; // Optional: Avatar Mask for firing animations
    public float defaultAvatarWeight = 0f; // Default weight for Avatar Mask
    public float firingAvatarWeight = 1.5f; // Weight when firing
    public EnemyAI enemyAI; // Reference to the EnemyAI script

    private float currentSpeed; // Current movement speed of the enemy

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (enemyAI == null)
        {
            enemyAI = GetComponent<EnemyAI>();
        }
    }

    void Update()
    {
        // Update the movement speed parameter
        currentSpeed = enemyAI.GetCurrentSpeed(); // A method in the EnemyAI script
        animator.SetFloat("Speed", currentSpeed);

        // Handle firing logic
        if (enemyAI.IsFiring()) // A method in the EnemyAI script
        {
            SetAvatarMaskWeight(firingAvatarWeight); // Increase weight while firing
            animator.SetBool("Shooting", true);
        }
        else
        {
            SetAvatarMaskWeight(defaultAvatarWeight); // Reset weight when not firing
            animator.SetBool("Shooting", false);
        }
    }


    void SetAvatarMaskWeight(float weight)
    {
        if (animator != null && avatarMask != null)
        {
            // Apply the Avatar Mask weight for firing animations
            animator.SetLayerWeight(1, weight); // Assuming layer 1 is where the Avatar Mask is applied
        }
    }
}


