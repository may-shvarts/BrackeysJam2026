using UnityEngine;
using DG.Tweening; // Required for DOTween

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(characterMovement))]
public class CharacterAnimation : MonoBehaviour
{
    [Header("Squash & Stretch Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    // Stretch: Makes character thinner and taller when jumping
    [SerializeField] private Vector3 stretchPunch = new Vector3(-0.3f, 0.3f, 0f); 
    // Squash (Bonus!): Makes character wider and shorter when landing
    [SerializeField] private Vector3 squashPunch = new Vector3(0.3f, -0.3f, 0f);
    
    [Header("Flicker Settings")]
    [SerializeField] private float flickerTimePerFlash = 0.1f;
    [SerializeField] private int flickerLoops = 8;

    private Animator _animator;
    private characterMovement _movement;
    private SpriteRenderer _spriteRenderer;
    private bool _wasGrounded;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<characterMovement>();
        
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnEnable()
    {
        EventManagement.OnTakingDamage += TriggerDamageFlicker;
    }

    private void OnDisable()
    {
        EventManagement.OnTakingDamage -= TriggerDamageFlicker;
    }

    private void Update()
    {
        // 1. קריאת הקלט מסקריפט התנועה
        float horizontalInput = _movement.HorizontalInput;

        // 2. עדכון האנימטור
        bool isWalking = Mathf.Abs(horizontalInput) > 0.1f;
        _animator.SetBool("isWalking", isWalking);

        // 3. הפיכת (Flip) הדמות בצורה בטוחה שלא דורסת את DOTween
        Vector3 currentScale = transform.localScale;
        float facingSign = Mathf.Sign(currentScale.x);

        if (horizontalInput > 0)
        {
            facingSign = 1f; // פונה ימינה
        }
        else if (horizontalInput < 0)
        {
            facingSign = -1f; // פונה שמאלה
        }

        // Apply flip while maintaining the absolute scale values currently driven by DOTween
        transform.localScale = new Vector3(Mathf.Abs(currentScale.x) * facingSign, currentScale.y, currentScale.z);

        // 4. Check for jumps and landings
        HandleSquashAndStretch();
    }

    private void HandleSquashAndStretch()
    {
        bool isGrounded = _movement.IsGrounded;

        // Character just left the ground (Jumping)
        if (!isGrounded && _wasGrounded)
        {
            PlayScaleEffect(stretchPunch);
        }
        // Character just landed back on the ground
        else if (isGrounded && !_wasGrounded)
        {
            PlayScaleEffect(squashPunch);
        }

        _wasGrounded = isGrounded; // Update state for the next frame
    }

    private void PlayScaleEffect(Vector3 punchAmount)
    {
        // Stop any currently running scale animations on this object to prevent glitching
        transform.DOKill();

        // Reset to base scale before punching, ensuring we preserve the facing direction
        float facingSign = Mathf.Sign(transform.localScale.x);
        transform.localScale = new Vector3(1f * facingSign, 1f, 1f);

        // Adjust the X punch so it squashes/stretches correctly regardless of which way we are facing
        Vector3 adjustedPunch = new Vector3(punchAmount.x * facingSign, punchAmount.y, punchAmount.z);

        // DOPunchScale adds an elastic "punch" to the scale and automatically returns it to normal
        transform.DOPunchScale(adjustedPunch, animationDuration, vibrato: 1, elasticity: 0.5f);
    }
    
    private void TriggerDamageFlicker()
    {
        if (_spriteRenderer == null) return;

        _spriteRenderer.DOKill();

        Color defaultColor = _spriteRenderer.color;
        defaultColor.a = 1f;
        _spriteRenderer.color = defaultColor;

        _spriteRenderer.DOFade(0.2f, flickerTimePerFlash)
            .SetLoops(flickerLoops, LoopType.Yoyo)
            .OnComplete(() => 
            {
                Color finalColor = _spriteRenderer.color;
                finalColor.a = 1f;
                _spriteRenderer.color = finalColor;
            });
    }
}