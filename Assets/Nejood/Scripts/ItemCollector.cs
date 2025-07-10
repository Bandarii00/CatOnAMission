using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int collectedItems = 0;
    private CatPlayerController playerController;

    void Start()
    {
        playerController = GetComponent<CatPlayerController>();
        if (playerController == null)
        {
            Debug.LogError("CatPlayerController not found on the player.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            collectedItems++;

            // Unlock dash once per collected item
            playerController.UnlockDash();

            Destroy(other.gameObject);
        }
    }
}
