using UnityEngine;
using StarterAssets;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;

    [Header("Fall Values")]
    [SerializeField] private float fallDamage = 7f;
    [SerializeField] private float minimumFallHeight = 3f;

    [Header("UI")]
    [SerializeField] private StatsBar healthBar;
    [SerializeField] private DeathScreen deathScreen;

    public bool IsAlive { get; private set; } = true;

    private void Start()
    {
        DisableRagadoll();

        // DISABLE CURSOR

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        healthBar.UpdateBar(health, maxHealth);
        if (health<=0 && IsAlive)
        {
            Die();
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
    }
    public void AddHealth(float addedHealth)
    {
        health += addedHealth;
    }
    public void Die()
    {
        EnableRagdoll();
        GetComponent<ThirdPersonController>().enabled = false;

        // ENABLE CURSOR
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        deathScreen.Show();
        IsAlive = false;
    }

    public void Land(float lastHeight)
    {
        float height = Vector3.Distance(Vector3.up * lastHeight, Vector3.up * transform.position.y);
        if (height<minimumFallHeight||lastHeight<transform.position.y)
        {
            return;
        }
        TakeDamage(fallDamage * height);
    }


    public void EnableRagdoll()
    {
        GetComponent<Animator>().enabled = false;
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            if (rb==GetComponent<Rigidbody>())
            {
                continue;
            }
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public void DisableRagadoll()
    {
        GetComponent<Animator>().enabled = true;
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            if (rb == GetComponent<Rigidbody>())
            {
                continue;
            }
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}
