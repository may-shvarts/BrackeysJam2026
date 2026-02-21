using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class characterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpVelocity = 20f;

    [Header("Ground Rule")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Jump Feel")]
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float fallMultiplier = 2.5f;      // faster fall
    [SerializeField] private float lowJumpMultiplier = 2f;     // short hop if button released early

    private bool _jumpHeld;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    private float _jumpBufferCounter;
    private bool _isGrounded;
    private bool _canMove = true;
    public float HorizontalInput => _moveInput.x;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        EventManagement.OnPlayerFreeze   += FreezePlayer;
        EventManagement.OnPlayerUnfreeze += UnfreezePlayer;
        EventManagement.RestartGame += UnfreezePlayer;
    }

    void OnDisable()
    {
        EventManagement.OnPlayerFreeze   -= FreezePlayer;
        EventManagement.OnPlayerUnfreeze -= UnfreezePlayer;
        EventManagement.RestartGame -= UnfreezePlayer;
    }

    private void FreezePlayer()
    {
        _canMove = false;
        _moveInput = Vector2.zero;
        _jumpBufferCounter = 0f;
        _rb.linearVelocity = Vector2.zero;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void UnfreezePlayer()
    {
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _canMove = true;
    }

    private void OnMove(InputValue value)
    {
        if (!_canMove) return;
        _moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (!_canMove) return;
        _jumpHeld = value.isPressed;
        if (value.isPressed)
            _jumpBufferCounter = jumpBufferTime;
    }
    private void FixedUpdate()
    {
        if (!_canMove) return;
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);

        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);

        if (_jumpBufferCounter > 0f && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpVelocity);
            _jumpBufferCounter = 0f;
        }

        if (_jumpBufferCounter > 0f)
            _jumpBufferCounter -= Time.fixedDeltaTime;
        // Better jump arc
        if (_rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (_rb.linearVelocity.y > 0 && !_jumpHeld)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
}