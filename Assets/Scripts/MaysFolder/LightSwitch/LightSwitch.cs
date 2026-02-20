using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("התמונה שתופיע אחרי הלחיצה")]
    [SerializeField] private Sprite newSprite; 
    
    private SpriteRenderer spriteRenderer;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isPlayerInRange && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ChangeSprite();
            DOVirtual.DelayedCall(2f, () =>
            {
                EventManagement.OnWin?.Invoke();
            });
        }
    }

    private void ChangeSprite()
    {
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}