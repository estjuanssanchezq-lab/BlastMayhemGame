using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour, IPickupReceiver
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float bombInventory = 0f;

    private Vector2 movement;

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);
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
}




