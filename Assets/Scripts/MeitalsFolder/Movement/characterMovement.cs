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

    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    private float _jumpBufferCounter;
    private bool _isGrounded;
    private bool _canMove = true;
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
    }
}