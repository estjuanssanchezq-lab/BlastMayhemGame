using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour, IPickupReceiver
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float bombInventory = 0f;
    [SerializeField] private Transform throwPoint; // Punto desde donde se lanzará la bomba
    [SerializeField] GameObject bombPrefab; // Prefab de la bomba a lanzar

    private Vector2 movement;

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);

        if(Input.GetKeyDown(KeyCode.X))
        {
            ThrowBomb();
        }
    }

    // Aumentar bombInventory al recoger una bomba
    public void AddBomb()
    {
        bombInventory++;
        Debug.Log("Bombas en inventario: " + bombInventory);
    }

    public void AddHealth()
    {
        Debug.Log("Salud aumentada");
    }

    public void AddBoomerang()
    {
        Debug.Log("Tipo de bomba ahora: Boomerang");
    }

    private void ThrowBomb()
    {
        if (bombInventory <= 0) return;
        bombInventory--;

        Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);

        Debug.Log("Bomba lanzada. Bombas restantes: " + bombInventory);
    }
}




