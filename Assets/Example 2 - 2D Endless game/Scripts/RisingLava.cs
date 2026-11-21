using UnityEngine;

// A lava area that rises from below and gets faster over time.
// Kills the player on contact, so standing still is no longer possible.
public class RisingLava : MonoBehaviour
{
    [SerializeField] Transform player;

    [Header("Speed")]
    [Tooltip("Rising speed at the start (units per second).")]
    [SerializeField] float startSpeed = 0.8f;
    [Tooltip("How much the speed increases every second.")]
    [SerializeField] float acceleration = 0.04f;
    [SerializeField] float maxSpeed = 6f;

    [Header("Catch Up")]
    [Tooltip("If the player is further above the lava than this, the lava jumps closer.")]
    [SerializeField] float maxDistanceBelowPlayer = 14f;

    float currentSpeed;

    void Start()
    {
        currentSpeed = startSpeed;
    }

    void Update()
    {
        // Time.deltaTime is 0 while paused, so the lava stops automatically
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;

        // Keep the lava close behind the player so it stays a threat
        float lavaTop = transform.position.y + transform.localScale.y / 2f;
        float minLavaTop = player.position.y - maxDistanceBelowPlayer;
        if (lavaTop < minLavaTop)
        {
            transform.position += Vector3.up * (minLavaTop - lavaTop);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Die();
        }
    }
}