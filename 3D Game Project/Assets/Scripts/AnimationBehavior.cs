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

        bool isSprinting = (horizontalInput != 0 || verticalInput > 0) && Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("Sprinting", isSprinting);

        HandleJumpingAndFalling();


        if (Input.GetButton("Fire1"))
        {
            animator.SetBool("Sprinting", false);
            SetAvatarMaskWeight(firingAvatarWeight);
        }
        else
        {
            SetAvatarMaskWeight(defaultAvatarWeight);
        }
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
