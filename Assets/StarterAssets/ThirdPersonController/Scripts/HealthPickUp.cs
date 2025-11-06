using UnityEngine;

public class HealthPickUp : Interactable
{
    [SerializeField] private float health = 50f;

    public override void Interact(GameObject player) {
        player.GetComponent<PlayerStats>().AddHealth(health);
        base.Interact(player);
    }

}
