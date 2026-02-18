using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerLives : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float invulnerableTime = 0.8f;

    [Header("Damage Layer")]
    [SerializeField] private LayerMask obstacleLayer;

    private int _currentLives;
    private float _invulnerableTimer;

    private Vector3 _startPosition;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _currentLives = maxLives;
        _rb = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_invulnerableTimer > 0f)
            _invulnerableTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsObstacle(other.gameObject))
            TakeDamage();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsObstacle(collision.gameObject))
            TakeDamage();
    }

    private bool IsObstacle(GameObject obj)
    {
        if (_invulnerableTimer > 0f)
            return false;
        return ((1 << obj.layer) & obstacleLayer) != 0;
    }

    private void TakeDamage()
    {
        _currentLives--;
        _invulnerableTimer = invulnerableTime;

        // Always respawn after a hit
        Respawn();

        // If no lives left, fire the "player died" event and reset lives
        if (_currentLives <= 0)
        {
            EventManagement.OnPlayerDied?.Invoke();
            _currentLives = maxLives;
        }
    }

    private void Respawn()
    {
        transform.position = _startPosition;
        _rb.linearVelocity = Vector2.zero;
    }
}