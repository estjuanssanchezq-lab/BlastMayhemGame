using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Vector3 originalScale;

    public GameObject bombPrefab;
    public Transform bombSpawnPoint;

    public int maxHealth = 3;
    private int currentHealth;

    private Rigidbody2D rb;
    private bool isGrounded;
    
    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        originalScale = transform.localScale;

        currentHealth = maxHealth;
    }

    void Update()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.A))
        {
        moveInput = -1f;
        direction = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            direction = 1;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
        ThrowBomb();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            isGrounded = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player murió");
        gameObject.SetActive(false);
    }

    void ThrowBomb()
    {
    if (bombPrefab == null)
    {
        Debug.LogWarning("Falta asignar bombPrefab en Player1");
        return;
    }

    if (bombSpawnPoint == null)
    {
        Debug.LogWarning("Falta asignar BombSpawnPoint en Player1");
        return;
    }

    GameObject bomb = Instantiate(
        bombPrefab,
        bombSpawnPoint.position,
        Quaternion.identity
    );

    Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();

    if (bombRb != null)
    {
        bombRb.AddForce(new Vector2(direction * 8f, 4f), ForceMode2D.Impulse);
    }
    }
}