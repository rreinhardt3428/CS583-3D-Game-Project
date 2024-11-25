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


   void Start()
   {
   //Set the controller variable to the relevant component
   playerController = GetComponent<CharacterController>();
   }

   void Update()
   {
   //Grab current inputs from the player
   horizontalInput = Input.GetAxis("Horizontal");
   verticalInput = Input.GetAxis("Vertical");
   //Sets player direction based on inputs
   Vector3 playerDirection = transform.forward * verticalInput + transform.right * horizontalInput;
   //Moves the player based on direction and speed
   playerController.Move(playerDirection * playerSpeed * Time.deltaTime);
   //Sets the sphere position based on character
   spherePosition = new Vector3(transform.position.x, transform.position.y - floorOffset, transform.position.z);
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
