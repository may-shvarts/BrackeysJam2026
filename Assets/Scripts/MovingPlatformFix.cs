using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformFix : MonoBehaviour
{
    private Vector3 previousPosition; 
    private HashSet<Transform> objectsOnPlatform = new HashSet<Transform>();

    void Start()
    {
        previousPosition = transform.position;
    }

    void FixedUpdate() 
    {
        Vector3 deltaPosition = transform.position - previousPosition;
        foreach (Transform obj in objectsOnPlatform)
        {
            obj.position += deltaPosition;
        }
        previousPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            objectsOnPlatform.Add(collision.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            objectsOnPlatform.Remove(collision.transform);
        }
    }
}