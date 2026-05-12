using UnityEngine;

// Esta clase controla todo lo relacionado con el jugador:
// movimiento, salto, doble salto, animaciones, vida y lanzamiento de bombas.
public class PlayerMovement : MonoBehaviour
{
    // Velocidad con la que se mueve el jugador hacia la izquierda o derecha.
    public float moveSpeed = 5f;

    // Fuerza con la que salta el jugador.
    public float jumpForce = 10f;

    // Guarda la escala original del personaje.
    // Esto sirve para voltearlo a izquierda/derecha sin cambiarle el tamaño.
    private Vector3 originalScale;

    // Prefab de la bomba que el jugador va a lanzar.
    // Se asigna desde el Inspector de Unity.
    public GameObject bombPrefab;

    // Punto desde donde aparece la bomba cuando se lanza.
    // Normalmente es un objeto hijo llamado BombSpawnPoint.
    public Transform bombSpawnPoint;

    // Vida máxima del jugador.
    // Como es public, se puede cambiar desde el Inspector.
    public int maxHealth = 3;

    // Vida actual del jugador.
    // Es private porque solo se maneja desde este script.
    private int currentHealth;

    // Referencia al Rigidbody2D del jugador.
    // Sirve para moverlo usando físicas.
    private Rigidbody2D rb;

    // Indica si el jugador está tocando el piso.
    // true = está en el piso.
    // false = está en el aire.
    private bool isGrounded;

    // Referencia al Animator del jugador.
    // Sirve para cambiar entre Idle, Run y Jump.
    private Animator animator;

    // Dirección hacia donde mira el jugador.
    // 1 = derecha.
    // -1 = izquierda.
    private int direction = 1;

    // Cantidad máxima de saltos permitidos.
    // 2 significa salto normal + doble salto.
    public int maxJumps = 2;

    // Cantidad de saltos que le quedan al jugador en este momento.
    private int jumpsRemaining;

    [Header("Controles")]

    // Tecla para moverse a la izquierda.
    // Player1 usa A.
    // Player2 puede usar LeftArrow desde el Inspector.
    public KeyCode leftKey = KeyCode.A;

    // Tecla para moverse a la derecha.
    // Player1 usa D.
    // Player2 puede usar RightArrow desde el Inspector.
    public KeyCode rightKey = KeyCode.D;

    // Tecla para saltar.
    // Player1 usa W.
    // Player2 puede usar UpArrow desde el Inspector.
    public KeyCode jumpKey = KeyCode.W;

    // Tecla para lanzar bomba.
    // Player1 usa X.
    // Player2 puede usar Space desde el Inspector.
    public KeyCode bombKey = KeyCode.X;

    // Start se ejecuta una sola vez cuando inicia el juego.
    void Start()
    {
        // Obtiene el Rigidbody2D que está puesto en el jugador.
        rb = GetComponent<Rigidbody2D>();

        // Obtiene el Animator que está puesto en el jugador.
        animator = GetComponent<Animator>();

        // Guarda el tamaño original del personaje.
        // Esto es importante porque después lo volteamos usando escala negativa.
        originalScale = transform.localScale;

        // Al iniciar, la vida actual será igual a la vida máxima.
        currentHealth = maxHealth;

        // Al iniciar, el jugador tiene todos sus saltos disponibles.
        jumpsRemaining = maxJumps;
    }

    // Update se ejecuta una vez por cada frame.
    // Aquí se revisan teclas y acciones constantes del jugador.
    void Update()
    {
        // Variable temporal para saber si el jugador se mueve.
        // -1 = izquierda.
        // 0 = quieto.
        // 1 = derecha.
        float moveInput = 0f;

        // Si se presiona la tecla de izquierda...
        if (Input.GetKey(leftKey))
        {
            // El jugador se moverá hacia la izquierda.
            moveInput = -1f;

            // Guardamos que está mirando hacia la izquierda.
            direction = -1;
        }

        // Si se presiona la tecla de derecha...
        if (Input.GetKey(rightKey))
        {
            // El jugador se moverá hacia la derecha.
            moveInput = 1f;

            // Guardamos que está mirando hacia la derecha.
            direction = 1;
        }

        // Aplicamos el movimiento horizontal al Rigidbody2D.
        // En X usamos la velocidad de movimiento.
        // En Y conservamos la velocidad actual para no afectar salto o caída.
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Le decimos al Animator si el jugador está corriendo.
        // Si moveInput es diferente de 0, significa que se está moviendo.
        animator.SetBool("isRunning", moveInput != 0);

        // Le decimos al Animator si el jugador está saltando.
        // Si NO está en el piso, entonces está en el aire.
        animator.SetBool("isJumping", !isGrounded);

        // Si el jugador se mueve hacia la derecha...
        if (moveInput > 0)
        {
            // Lo ponemos mirando hacia la derecha.
            // Mathf.Abs asegura que X sea positiva.
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        // Si el jugador se mueve hacia la izquierda...
        else if (moveInput < 0)
        {
            // Lo ponemos mirando hacia la izquierda.
            // La X negativa voltea visualmente el sprite.
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }

        // Si se presiona la tecla de salto y todavía quedan saltos disponibles...
        if (Input.GetKeyDown(jumpKey) && jumpsRemaining > 0)
        {
            // Aplicamos fuerza de salto en Y.
            // La X se conserva para que pueda saltar mientras se mueve.
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Restamos un salto disponible.
            jumpsRemaining--;
        }

        // Esta tecla H es solo para probar daño.
        // Cuando el sistema de bombas haga daño, esta parte se puede borrar.
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Le quita 1 punto de vida al jugador.
            TakeDamage(1);
        }

        // Si se presiona la tecla de bomba...
        if (Input.GetKeyDown(bombKey))
        {
            // Lanza una bomba.
            ThrowBomb();
        }
    }

    // Esta función se ejecuta cuando el jugador empieza a tocar otro collider.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el objeto con el que chocó se llama Ground...
        if (collision.gameObject.name == "Ground")
        {
            // Marcamos que el jugador está en el piso.
            isGrounded = true;

            // Restauramos los saltos disponibles.
            // Esto permite volver a hacer salto y doble salto.
            jumpsRemaining = maxJumps;
        }
    }

    // Esta función se ejecuta cuando el jugador deja de tocar otro collider.
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Si dejó de tocar el objeto llamado Ground...
        if (collision.gameObject.name == "Ground")
        {
            // Marcamos que ya no está en el piso.
            isGrounded = false;
        }
    }

    // Función pública para recibir daño.
    // Otros scripts, como una bomba, pueden llamar esta función.
    public void TakeDamage(int damage)
    {
        // Restamos el daño recibido a la vida actual.
        currentHealth -= damage;

        // Mostramos la vida actual en la consola.
        Debug.Log("Vida actual: " + currentHealth);

        // Si la vida llega a 0 o menos...
        if (currentHealth <= 0)
        {
            // El jugador muere.
            Die();
        }
    }

    // Función que se ejecuta cuando el jugador muere.
    void Die()
    {
        // Mensaje en consola para saber que murió.
        Debug.Log("Player murió");

        // Desactiva el objeto del jugador.
        // Visualmente desaparece del juego.
        gameObject.SetActive(false);
    }

    // Función encargada de lanzar la bomba.
    void ThrowBomb()
    {
        // Si no se asignó el prefab de la bomba en el Inspector...
        if (bombPrefab == null)
        {
            // Mostramos una advertencia y detenemos la función.
            Debug.LogWarning("Falta asignar bombPrefab en Player1");
            return;
        }

        // Si no se asignó el punto desde donde sale la bomba...
        if (bombSpawnPoint == null)
        {
            // Mostramos una advertencia y detenemos la función.
            Debug.LogWarning("Falta asignar BombSpawnPoint en Player1");
            return;
        }

        // Creamos una bomba en la posición del BombSpawnPoint.
        GameObject bomb = Instantiate(
            bombPrefab,
            bombSpawnPoint.position,
            Quaternion.identity
        );

        // Buscamos si la bomba tiene Rigidbody2D.
        Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();

        // Si la bomba sí tiene Rigidbody2D...
        if (bombRb != null)
        {
            // Le damos fuerza a la bomba.
            // direction controla si sale hacia izquierda o derecha.
            // 8f es la fuerza horizontal.
            // 4f es la fuerza vertical.
            bombRb.AddForce(new Vector2(direction * 8f, 4f), ForceMode2D.Impulse);
        }
    }
}