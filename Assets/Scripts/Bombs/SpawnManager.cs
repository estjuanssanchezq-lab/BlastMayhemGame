using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject bombPrefab; // Prefab de la bomba 
    [SerializeField] private GameObject healthPrefab; // Prefab de la vida
    [SerializeField] private GameObject stickyPrefab; // Prefab del sticky

    [Header("Spawn Points")]
    [SerializeField] private Transform[] fallingSpawnPoints;
    [SerializeField] private Transform[] floatingSpawnPoints;

    [Header("Repeat Times")]
    [SerializeField] private float bombRepeatTime = 5f;
    [SerializeField] private float healthRepeatTime = 8f;
    [SerializeField] private float stickyRepeatTime = 10f;

    [SerializeField] private LayerMask blockedSpawnLayers; // Capa para detectar obstáculos en los puntos de spawn
    [SerializeField] private float checkRadius = 0.6f; // Radio para verificar si el punto de spawn está bloqueado

    void Start()
    {
        // InvokeRepeating necesita llamar a metodos separados. En cada metodo se reutiliza el metodo Spawn, que se encarga de instanciar un pickup aleatorio en un punto de spawn aleatorio.
        InvokeRepeating(nameof(SpawnBomb), 2f, bombRepeatTime); // Inicia el spawn después de 2 segundos y luego se repite cada repeatTime segundos
        InvokeRepeating(nameof(SpawnHealth), 5f, healthRepeatTime); // Inicia el spawn después de 5 segundos y luego se repite cada repeatTime segundos
        InvokeRepeating(nameof(SpawnSticky), 3f, stickyRepeatTime); // Inicia el spawn después de 3 segundos y luego se repite cada repeatTime segundos
    }

    private void SpawnBomb()
    {
        Spawn(bombPrefab, fallingSpawnPoints);
    }
    private void SpawnHealth()
    {
        Spawn(healthPrefab, floatingSpawnPoints);
    }
    private void SpawnSticky()
    {
        Spawn(stickyPrefab, floatingSpawnPoints);
    }

    void Spawn(GameObject prefab, Transform[] points)
    {
        if (prefab == null || points.Length == 0) return; // Verifica que el prefab y los puntos de spawn estén asignados

        int randomIndex = Random.Range(0, points.Length); // Selecciona un índice aleatorio para elegir un punto de spawn
        Vector2 spawnPosition = points[randomIndex].position;

        bool blocked = Physics2D.OverlapCircle(
                        spawnPosition, // centro del circulo
                        checkRadius, // radio del circulo
                        blockedSpawnLayers); // layer que se va a verificar

        if (blocked)
        {
            Debug.Log("Punto de spawn bloqueado, no se puede generar el pickup.");
            return; // Si el punto de spawn está bloqueado, no genera el pickup
        }

        Instantiate(prefab, // Instancia el prefab del pickup
                    spawnPosition, // Posición del punto de spawn seleccionado aleatoriamente
                    Quaternion.identity);
    }


    private void OnDrawGizmos() // Dibuja un gizmo para visualizar el área de verificación de spawn en el editor
    {
        Gizmos.color = Color.gray4;

        foreach (Transform point in floatingSpawnPoints)
        {
            Gizmos.DrawWireSphere(point.position, checkRadius);
        }

        foreach (Transform point in fallingSpawnPoints)
        {
            Gizmos.DrawWireSphere(point.position, checkRadius);
        }
    }
}
