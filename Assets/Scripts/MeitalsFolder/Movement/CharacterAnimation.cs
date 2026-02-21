using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(characterMovement))]
public class CharacterAnimation : MonoBehaviour
{
    private Animator _animator;
    private characterMovement _movement;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<characterMovement>();
    }

    private void Update()
    {
        // 1. קריאת הקלט מסקריפט התנועה
        float horizontalInput = _movement.HorizontalInput;

        // 2. עדכון האנימטור (אם הערך המוחלט גדול מ-0.1, השחקן זז)
        bool isWalking = Mathf.Abs(horizontalInput) > 0.1f;
        _animator.SetBool("isWalking", isWalking);

        // 3. הפיכת (Flip) הדמות לכיוון ההליכה
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // פונה ימינה
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // פונה שמאלה
        }
    }
}