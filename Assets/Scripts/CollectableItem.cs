using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MainScript.instance.ItemCollected();  // Notify MainScript that an item was collected
            Destroy(gameObject);  // Destroy or deactivate the collectible item
        }
    }
}