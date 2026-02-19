using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerLives : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] public static int MaxLives { get; set; } = 3;
    [SerializeField] private float invulnerableTime = 0.8f;

    [Header("Damage Layer")]
    [SerializeField] private LayerMask obstacleLayer;

    private int _currentLives;
    private float _invulnerableTimer;

    private Vector3 _startPosition;
    private Rigidbody2D _rb;
    private void Awake()
    {
        _currentLives = MaxLives;
        _rb = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
    }

    private void OnEnable()
    {
        EventManagement.RestartGame += Restart;
    }

    private void OnDisable()
    {
        EventManagement.RestartGame -= Restart;
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
        EventManagement.OnLivesChanged?.Invoke(_currentLives);

        // Always respawn after a hit
        Respawn();

        // If no lives left, fire the "player died" event and reset lives
        if (_currentLives <= 0)
        {
            EventManagement.OnPlayerDied?.Invoke();
            _currentLives = MaxLives;
            EventManagement.OnLivesChanged?.Invoke(_currentLives);
        }
    }

    private void Restart()
    {
        _currentLives = MaxLives;
        Respawn();
    }
    private void Respawn()
    {
        transform.position = _startPosition;
        _rb.linearVelocity = Vector2.zero;
    }
}