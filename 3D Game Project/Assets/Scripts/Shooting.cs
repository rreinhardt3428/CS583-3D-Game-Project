using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun1Shooting : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera thirdPersonCamera;
    public float fireRate = 0.5f;
    public float range = 1000f; 
    public int damage = 10; // can be changed for each gun in inspector 
    private float timer = 0f;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= timer)
        {
            timer = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        Transform cameraTransform = thirdPersonCamera.VirtualCameraGameObject.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {

            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, .5f);
            }
            Debug.Log($"Hit {hit.collider.name} at {hit.point}");
        }

        else
        {
            Debug.Log("Missed");
        }

        
    }
}
