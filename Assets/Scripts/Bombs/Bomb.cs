using UnityEngine;

// Este script es para la bomba LANZADA, no para la bomba que se recoge.
public class Bomb : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private BombState currentState = BombState.Pickup;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D groundCollider;
    [SerializeField] private Collider2D playerDetector;

    [Header("Settings")]
    [SerializeField] private float restingVelocityThreshold = 0.2f; // Velocidad mínima para considerar que la bomba está en reposo
    [SerializeField] private int damage = 1; // Daño que la bomba inflige al jugador

    private bool collected = false;
    private bool isGrounded = false; // La bomba debe estar lenta y en el suelo para volver a ser recogida.

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == BombState.Thrown)
        {
                CheckIfResting();
        }
    }

    private void CheckIfResting()
    {
        if (isGrounded && rb.linearVelocity.magnitude <= restingVelocityThreshold) // Si la bomba está en el suelo y su velocidad es lo suficientemente baja, se considera que está en reposo
        {
            SetRestingState();
        }
    }
    private void SetRestingState()
    {
        currentState = BombState.Resting;

        rb.linearVelocity = Vector2.zero;
        playerDetector.enabled = false; // Activar el detector de jugadores para permitir que la bomba sea recogida
        Debug.Log("Bomba en estado Resting. Puede volver a ser recogida");
        Debug.Log("PlayerDetector enabled: " + playerDetector.enabled);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (currentState == BombState.Thrown)
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();

            if (player == null) return;

            player.TakeDamage(damage);

            Debug.Log("Bomba golpeó al jugador. Infligiendo daño: " + damage);

            Explode();

            return;
        }

        if (currentState == BombState.Pickup || currentState == BombState.Resting)
        {
            if (collected) return;

            IPickupReceiver receiver = collision.GetComponent<IPickupReceiver>();

            if (receiver == null) return;

            collected = true;

            Debug.Log("Bomba recogida");

            receiver.AddBomb();

            Collect();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground") // Si la bomba colisiona con el suelo, se considera que está en el suelo (grounded).
        {
            isGrounded = true;
        }
    }
    public void Collect()
    {
        animator.SetTrigger("Collect");

        rb.linearVelocity = Vector2.zero; // Detener la bomba
        rb.bodyType = RigidbodyType2D.Kinematic; // Hacer que la bomba no sea afectada por la física

        //groundCollider.enabled = false; // Desactivar el collider del suelo para evitar colisiones mientras se anima
        //playerDetector.enabled = false; // Desactivar el detector de jugadores para evitar recoger la bomba mientras se anima
    }

    public void Throw(Vector2 direction, float force)
    {
        collected = false;
        currentState = BombState.Thrown;
        isGrounded = false; // La bomba no está en el suelo al ser lanzada

        rb.bodyType = RigidbodyType2D.Dynamic; // Hacer que la bomba sea afectada por la física
        rb.linearVelocity = Vector2.zero; // Reiniciar la velocidad antes de aplicar la fuerza

        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse); // Aplicar la fuerza para lanzar
    }

    private void Explode()
    {
        Debug.Log("Bomba explotó");

        rb.linearVelocity = Vector2.zero; // Detener la bomba al explotar
        rb.bodyType = RigidbodyType2D.Kinematic; // Hacer que la bomba no sea afectada por la física al explotar

        // Desactivar colliders para evitar que la bomba siga interactuando con el jugador o el suelo después de explotar
        groundCollider.enabled = false; 
        playerDetector.enabled = false;

        // Faltaria agregar el animator.SetTrigger("Explode")
        //Destroy(gameObject); // Destruir la bomba después de explotar

        animator.SetTrigger("Explode");
    }
}

