using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static Vector2 playerPosition;

    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        playerPosition =
            transform.position;

        movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }


    private void FixedUpdate()
    {
        rb.linearVelocity =
            movement * moveSpeed;
    }


    public void MultiplySpeed(float multiplier)
    {
        moveSpeed *= multiplier;
    }
}