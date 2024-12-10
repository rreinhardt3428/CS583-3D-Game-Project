using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBehavior : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    float horizontalInput;
    float verticalInput;
    public AvatarMask avatarMask; // Reference to the Avatar Mask

    private float defaultAvatarWeight = 0f; // Default weight of Avatar Mask (can be changed)
    private float firingAvatarWeight = 1.5f; // Higher weight when firing (set as per your requirement)

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get movement inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Update animator parameters for movement
        animator.SetFloat("Horizontal Input", horizontalInput);
        animator.SetFloat("Vertical Input", verticalInput);

        // Handle sprinting
        bool isSprinting = Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("Sprinting", isSprinting);

        // Update jumping and falling logic
        HandleJumpingAndFalling();

        // Handle firing logic
        if (Input.GetButton("Fire1"))
        {
            animator.SetBool("Sprinting", false); // Stop sprinting when firing
            SetAvatarMaskWeight(firingAvatarWeight); // Increase weight while firing
        }
        else
        {
            SetAvatarMaskWeight(defaultAvatarWeight); // Reset weight when not firing
        }
    }

    void HandleJumpingAndFalling()
    {
        float verticalSpeed = rb.velocity.y;

        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(verticalSpeed) < 0.1f)
        {
            // Player initiated a jump
            animator.SetTrigger("Jumping Up");
        }

        if (verticalSpeed > 0.1f)
        {
            // Player is moving upward
            animator.ResetTrigger("Landing");
        }
        else if (verticalSpeed < -0.1f)
        {
            // Player is falling
            animator.ResetTrigger("Jumping Up");
        }
        else
        {
            if (Mathf.Abs(verticalSpeed) < 0.1f && !animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping Up"))
            {
                animator.SetTrigger("Landing"); // Trigger landing animation
            }
        }
    }

    void SetAvatarMaskWeight(float weight)
    {
        if (animator != null && avatarMask != null)
        {
            // Set the weight of the Avatar Mask to make certain parts of the body more active
            animator.SetLayerWeight(1, weight); // Assuming layer 1 is where the Avatar Mask is applied
        }
    }
}
