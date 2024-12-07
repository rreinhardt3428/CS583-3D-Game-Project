using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   //Create values for player speed and getting movement inputs (WASD)
   public float playerSpeed;
   float horizontalInput;
   float verticalInput;
   //Create a variable for the character controller component
   CharacterController playerController;
   //Variables needed for sphere scan to determine if the model is colliding with the floor
   public float floorOffset;
   public LayerMask floorMask;
   Vector3 spherePosition;
   //Set gravity and velocity for the player
   public float gravity = -20.0f;
   Vector3 velocity;
   //Set jump force
   public float jumpForce = 10.0f;
   //Set sprint speed
   public float sprintSpeed;
   //Placeholder speed
   float baseSpeed;


   void Start()
   {
   //Set the controller variable to the relevant component
   playerController = GetComponent<CharacterController>();
   //Grab the base speed of the player
   baseSpeed = playerSpeed;
   }

   void Update()
   {
   //Grab current inputs from the player
   horizontalInput = Input.GetAxisRaw("Horizontal");
   verticalInput = Input.GetAxisRaw("Vertical");
   //Checks to see if the player is moving forward and pressing LeftShift
   if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
   {
      //Makes sure the player isnt moving side to side
      if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
      {
            //Changes playerSpeed to sprintSpeed for direction calculation
            playerSpeed = sprintSpeed;
      } else {
            playerSpeed = baseSpeed;
      }
   } else {
      playerSpeed = baseSpeed;
   }
   //Sets player direction based on inputs
   Vector3 playerDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
   //Moves the player based on direction and speed
   playerController.Move(playerDirection * playerSpeed * Time.deltaTime);
   //Sets the sphere position based on character
   spherePosition = new Vector3(transform.position.x, transform.position.y - floorOffset, transform.position.z);
   //Input used for jumping which adds a y velocity if the player presses the corresponding button
   if(Input.GetKeyDown(KeyCode.Space))
   {
      //Player can only jump if theyre currently grounded
      if(Physics.CheckSphere(spherePosition, playerController.radius - 0.05f, floorMask))
      {
            velocity.y += jumpForce;
      } 
   }

   //Checks to see whether the sphere is colliding with the floor and if not applies gravity / velocity to the player
   if(!Physics.CheckSphere(spherePosition, playerController.radius - 0.05f, floorMask))
   {
         velocity.y += gravity * Time.deltaTime;
   } else if(velocity.y < 0){
         velocity.y = -2;
   }
   //Moves player towards the floor if in the air
   playerController.Move(velocity * Time.deltaTime);
   }

   
}
