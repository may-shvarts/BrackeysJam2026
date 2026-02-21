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
    [SerializeField] private bool isWinActivator;
    [SerializeField] private Transform originalLocation;
    [SerializeField] private Transform newSpriteLocation;
    
    private SpriteRenderer _spriteRenderer;
    private Sprite _originalSprite;
    private bool _isPlayerInRange = false;
    
    private Tween _winTween;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalSprite = _spriteRenderer.sprite;
    }
    
    private void OnEnable()
    {
        EventManagement.RestartGame += ResetInteractable;
    }

    private void OnDisable()
    {
        EventManagement.RestartGame -= ResetInteractable;
    }
    
    private void Update()
    {
        if (_isPlayerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            EventManagement.OnLightInteracted?.Invoke();
            ChangeSprite();
            if (isWinActivator)
            {
                _winTween = DOVirtual.DelayedCall(2f, () => { EventManagement.OnWin?.Invoke(); });
            }
        }
    }

    private void ResetInteractable()
    {
        if (_spriteRenderer != null && _originalSprite != null)
        {
            _spriteRenderer.sprite = _originalSprite;
            this.transform.position = originalLocation.position;
        }
        _winTween?.Kill();
    }
    
    private void ChangeSprite()
    {
        if (newSprite != null)
        {
            _spriteRenderer.sprite = newSprite;
            this.transform.position = newSpriteLocation.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            EventManagement.OnLightHoverEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInRange = false;
            EventManagement.OnLightHoverExit?.Invoke();
        }
    }
}