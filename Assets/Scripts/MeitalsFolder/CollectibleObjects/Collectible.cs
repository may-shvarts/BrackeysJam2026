using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void Awake()
    {
        EventManagement.RestartGame += ResetCollectible;
    }
    private void OnDestroy()
    {
        EventManagement.RestartGame -= ResetCollectible;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollectibles player = other.GetComponent<PlayerCollectibles>();

        if (player != null)
        {
            player.Collect();
            gameObject.SetActive(false);
        }
    }
    private void ResetCollectible()
    {
        gameObject.SetActive(true);
    }
}