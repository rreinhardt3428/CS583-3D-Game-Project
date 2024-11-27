using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingGun : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform rightHand;  // Assign this to the character's right hand bone or attachment point
    public GameObject gunPrefab; // This should be the gun prefab, not the model

    private GameObject gun;
    void Start()
    {
        // Instantiate the gun at the character's position (or any position you want)
        gun = Instantiate(gunPrefab, rightHand.position, rightHand.rotation);

        // Parent the gun to the right hand
        gun.transform.SetParent(rightHand);

        // Adjust the gun's position and rotation to avoid clipping
        gun.transform.localPosition = new Vector3(-0.156f, 0.054f, -0.005f);  // Adjust these values
        gun.transform.localRotation = Quaternion.Euler(-9.031f, 94.698f, -5.362f);  // Adjust this rotation
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
