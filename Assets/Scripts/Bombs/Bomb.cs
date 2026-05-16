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
    [SerializeField] private float restingVelocityThreshold = 1f; // Velocidad mínima para considerar que la bomba está en reposo
    [SerializeField] private int damage = 1; // Daño que la bomba inflige al jugador
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer para cambiar la apariencia de la bomba si es necesario

    [Header("Movements")]
    private BombParabolicMovement parabolicMovement;
    [SerializeField] private Color movingColor = new Color(0.8f, 0.3f, 0.3f);

    private bool collected = false;
    private bool isGrounded = false; // La bomba debe estar lenta y en el suelo para volver a ser recogida.
    private Color originalColor; // Para almacenar el color original del sprite de la bomba

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        parabolicMovement = GetComponent<BombParabolicMovement>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Start()
    {
        if (currentState == BombState.Pickup)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == BombState.Thrown)
        {
                CheckIfResting();
        }
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
        Debug.Log("Colisión con: " + collision.gameObject.name);
        Bomb otherBomb = collision.gameObject.GetComponentInParent<Bomb>();

        if (otherBomb != null)
        {
            Debug.Log("Detectó otra bomba");
            if (currentState == BombState.Thrown && otherBomb.isThrown()) 
            { 
                Explode();
                otherBomb.Explode();
                return;
            }
        }

        if (collision.gameObject.CompareTag("Ground")) // Si la bomba colisiona con el suelo, se considera que está en el suelo (grounded).
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
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
        spriteRenderer.color = originalColor;
        parabolicMovement.StopMovement(); // Detenemos la simulación manual

        Debug.Log("Bomba en estado Resting. Puede volver a ser recogida");
    }

    public void Collect()
    {
        animator.SetTrigger("Collect");

        parabolicMovement.StopMovement(); // Detiene la simulacion manual
        rb.bodyType = RigidbodyType2D.Kinematic; // Hacer que la bomba no sea afectada por la física
    }

    public void Throw(Vector2 direction, float initialSpeed)
    {
        collected = false;
        currentState = BombState.Thrown;
        isGrounded = false; // La bomba no está en el suelo al ser lanzada

        spriteRenderer.color = movingColor; // Cambia el color de la bomba para indicar que está en estado Thrown

        rb.bodyType = RigidbodyType2D.Dynamic;

        Vector2 initialVelocity = direction.normalized * initialSpeed;
        parabolicMovement.Launch(initialVelocity);
    }

    public void Explode()
    {
        Debug.Log("Bomba explotó");

        parabolicMovement.StopMovement(); // Detiene la simulacion manual
        // Aqui van los demas movimientos...

        rb.bodyType = RigidbodyType2D.Kinematic; // Hacer que la bomba no sea afectada por la física al explotar
        spriteRenderer.color = originalColor;
        animator.SetTrigger("Explode");
    }

    public bool isThrown()
    {
        return currentState == BombState.Thrown;
    }


}

