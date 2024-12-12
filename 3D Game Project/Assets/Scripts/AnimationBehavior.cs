using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AnimationBehavior : MonoBehaviour
{
    Animator animator;
    float horizontalInput;
    float verticalInput;
    public AvatarMask avatarMask;

    private float defaultAvatarWeight = 0f;
    private float firingAvatarWeight = 1.5f;

    private PlayerMovement playerMovement;
    private bool isReloading = false;


    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>(); // Assuming PlayerMovement is on the same object
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal Input", horizontalInput);
        animator.SetFloat("Vertical Input", verticalInput);

        bool isSprinting = (horizontalInput != 0 || verticalInput > 0) && Input.GetKey(KeyCode.LeftShift) && animator.GetBool("Reloading") == false;
        animator.SetBool("Sprinting", isSprinting);

        HandleJumpingAndFalling();
        HandleReloadAnimation();
        HandleFiringAnimation();
    }

    private void HandleFiringAnimation()
    {
        if (Input.GetButton("Fire1"))
        {
            animator.SetBool("Sprinting", false);
            animator.SetBool("Shooting", true);
            SetAvatarMaskWeight(firingAvatarWeight); // Apply the firing layer weight
        }
        else
        {
            // Don't change the weight when reloading is happening
            if (!isReloading)
            {
                SetAvatarMaskWeight(defaultAvatarWeight); // Reset to default if not reloading
            }
        }
    }

    private void HandleReloadAnimation()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading) // Ensure no reload during an active reload
        {
            // Trigger the reload animation
            animator.SetBool("Sprinting", false);
            animator.SetTrigger("Reload");
            animator.SetBool("Reloading", true);

            // Set reloading flag to true
            isReloading = true;

            // Apply the upper body layer for reload animation
            SetAvatarMaskWeight(firingAvatarWeight); // Apply full weight for the upper body layer during reload
            // Start coroutine to wait for reload animation to finish
            StartCoroutine(WaitForReloadToFinish());
        }
    }

    private IEnumerator WaitForReloadToFinish()
    {
        // Wait for the reload animation to finish (in layer 1)
        AnimatorStateInfo reloadStateInfo = animator.GetCurrentAnimatorStateInfo(1); // Assuming reload is on layer 1
        float reloadDuration = 1.6f;

        // Wait for the entire duration of the reload animation
        yield return new WaitForSeconds(reloadDuration);

        // Reset parameters and allow other actions like firing
        animator.SetBool("Reloading", false); // Reset Reloading parameter
        isReloading = false; // Allow firing again

        // Ensure to reset the layer weight to default after reload completes
        SetAvatarMaskWeight(defaultAvatarWeight); // Reset mask weight after reloading
    }

    private void HandleJumpingAndFalling()
    {
        // Accessing private velocity and gravity from PlayerMovement using reflection
        FieldInfo velocityField = typeof(PlayerMovement).GetField("velocity", BindingFlags.NonPublic | BindingFlags.Instance);

        if (velocityField != null)
        {
            Vector3 velocity = (Vector3)velocityField.GetValue(playerMovement);

            bool isGrounded = IsGrounded();
            animator.SetBool("Grounded", isGrounded);

            // Set the 'isJumping' parameter based on the grounded state and vertical velocity
            animator.SetBool("Jumping", !isGrounded && velocity.y > 0.1f); // Jump loop animation

            if (isGrounded)
            {
                // Reset jump/fall states when grounded
                animator.SetBool("Jumping", false);
                animator.SetFloat("MoveSpeed", (horizontalInput != 0 || verticalInput != 0) ? (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f) : 0f);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    private void SetAvatarMaskWeight(float weight)
    {
        if (animator != null && avatarMask != null)
        {
            animator.SetLayerWeight(1, weight); // Assuming layer 1 is where the Avatar Mask is applied
        }
    }
}
