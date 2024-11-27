using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun1Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform gunPoint;
    public Cinemachine.CinemachineVirtualCamera thirdPersonCamera;
    public float bulletForce = 20f;
    public float fireRate = 0.5f;
    public int damage = 10; // can be changed for each gun in inspector 
    private float timer = 0f;

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
        Transform cameraTransform = thirdPersonCamera.VirtualCameraGameObject.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        Vector3 shootDirection;

        if (Physics.Raycast(ray, out hit))
        {
            shootDirection = (hit.point - gunPoint.position).normalized;
        }
        else
        {
            Vector3 targetPoint = cameraTransform.position + cameraTransform.forward * 1000f;
            shootDirection = (targetPoint - gunPoint.position).normalized;
        }
        
        Quaternion bulletRotation = Quaternion.LookRotation(shootDirection) * Quaternion.Euler(90, 0, 0);
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, bulletRotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            bulletRb.AddForce(shootDirection * bulletForce, ForceMode.Impulse);
        }

        Destroy(bullet, 3f);
    }
}
