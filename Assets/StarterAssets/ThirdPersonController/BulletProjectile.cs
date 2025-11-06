using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    //[SerializeField] private Transform vfxHitGreen;
    //[SerializeField] private Transform vfxHitRed;

    private Rigidbody bulletRigidbody;
    public float damage = 25f;
    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 170f;
        bulletRigidbody.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �arpt���m�z �eyin tag'i "Enemy" mi?
        if (other.CompareTag("Enemy"))
        {
            // Evet, "Enemy" tag'i var. Ondan EnemyAI script'ini almay� dene
            EnemyAI enemy = other.GetComponent<EnemyAI>();

            // Script'i bulabildiysek...
            if (enemy != null)
            {
                // ...ona hasar ver!
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject,1f);
    }
    

}