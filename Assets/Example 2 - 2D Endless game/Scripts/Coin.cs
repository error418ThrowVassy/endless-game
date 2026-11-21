using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int value = 5;
    [SerializeField] float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        GameManager.Instance.AddCoin(value);
        AudioManager.PlayCoin();
        Destroy(gameObject);
    }
}