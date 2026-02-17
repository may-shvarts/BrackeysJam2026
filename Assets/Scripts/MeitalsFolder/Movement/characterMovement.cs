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

    [Range(0f, 1f)]
    [SerializeField] private float minGroundNormalY = 0.6f;

    [Header("Jump Feel")]
    [SerializeField] private float jumpBufferTime = 0.12f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private float _jumpBufferCounter;
    private bool _isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
            _jumpBufferCounter = jumpBufferTime;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (_jumpBufferCounter > 0f && _isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            _jumpBufferCounter = 0f;
        }

        if (_jumpBufferCounter > 0f)
            _jumpBufferCounter -= Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayers) == 0)
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            var contact = collision.GetContact(i);
            if (contact.normal.y >= minGroundNormalY)
            {
                _isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayers) == 0)
            return;

        _isGrounded = false;
    }
}