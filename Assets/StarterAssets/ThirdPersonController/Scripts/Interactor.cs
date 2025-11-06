using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class Interactor : MonoBehaviour
{
    private PlayerStats stats;
    private Interactable interactable;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactText;

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
    }
    private void Update()
    {
        interactText.SetActive(interactable != null);

        if (Input.GetKeyDown(interactKey)&&stats.IsAlive)
        {
            Interact();
        }
    }
    private void Interact()
    {
        if (interactable==null)
        {
            return;
        }
        interactable.Interact(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        Interactable foundInteractable = other.GetComponent<Interactable>();
        if (foundInteractable!=null)
        {
            interactable = foundInteractable;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Interactable foundInteractable = other.GetComponent<Interactable>();
        if (foundInteractable==interactable)
        {
            interactable = null;
        }
    }
}
