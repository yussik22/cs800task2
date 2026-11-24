using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 direction =
            (PlayerMovement.playerPosition - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed;
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}