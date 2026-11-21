using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameManager gameManager;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundedRaycastStartPoint;

    [Header("Player Stats")]
    [SerializeField] float jumpForceSide = 8f;
    [SerializeField] float jumpForceUp = 12f;
    [SerializeField] int maxJumps = 2;
    [SerializeField] float defaultGravityScale = 2f;
    [SerializeField] float wallClingGravityScale = 0.1f;
    [SerializeField] float groundCheckDistance = 0.6f;
    [Tooltip("Set it to ignore the player layer")]
    [SerializeField] LayerMask groundedCheckLayerMask;
    [Tooltip("Tolerance for the 'moving away from wall' velocity check, to absorb tiny residual velocities the physics solver leaves after resolving a wall collision.")]
    [SerializeField] float wallStickVelocityTolerance = 0.5f;

    [Header("Effects")]
    [SerializeField] GameObject deathEffect;

    int jumpsRemaining;
    bool wasGroundedLastFrame;
    bool isDead;

    void Start()
    {
        jumpsRemaining = maxJumps;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (gameManager.IsGameOver || gameManager.IsPaused) return;

        bool clingingLeft = IsGrounded(Vector2.left) && rb.linearVelocity.x <= wallStickVelocityTolerance;
        bool clingingRight = IsGrounded(Vector2.right) && rb.linearVelocity.x >= -wallStickVelocityTolerance;

        if (clingingLeft || clingingRight)
        {
            rb.gravityScale = wallClingGravityScale;

            if (!wasGroundedLastFrame)
            {
                jumpsRemaining = maxJumps;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                wasGroundedLastFrame = true;
            }
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
            wasGroundedLastFrame = false;
        }

        if (!clingingLeft && Input.GetMouseButtonDown(0) && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(-jumpForceSide, jumpForceUp);
            jumpsRemaining--;
        }
        else if (!clingingRight && Input.GetMouseButtonDown(1) && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(jumpForceSide, jumpForceUp);
            jumpsRemaining--;
        }
    }

    bool IsGrounded(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(groundedRaycastStartPoint.position, direction, groundCheckDistance, groundedCheckLayerMask);
        return hit && hit.collider.GetComponent<Obstacle>() == null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Obstacle>() != null)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        rb.gravityScale = defaultGravityScale;
        rb.freezeRotation = false;
        rb.AddTorque(25f);
        gameManager.OnPlayerCollidedWithObstacle();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundedRaycastStartPoint.position, Vector2.left * groundCheckDistance);
        Gizmos.DrawRay(groundedRaycastStartPoint.position, Vector2.right * groundCheckDistance);
    }
}