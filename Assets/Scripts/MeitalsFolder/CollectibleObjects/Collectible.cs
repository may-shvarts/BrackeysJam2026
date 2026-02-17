using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollectibles player = other.GetComponent<PlayerCollectibles>();

        if (player != null)
        {
            player.Collect();
            Destroy(gameObject);
        }
    }
}