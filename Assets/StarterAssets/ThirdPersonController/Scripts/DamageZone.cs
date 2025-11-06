using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damage = 40f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();

        if (stats!=null)
        {
            stats.TakeDamage(damage);
        }
    }
}
