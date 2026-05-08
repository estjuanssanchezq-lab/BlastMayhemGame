using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour, IPlayerPickupReceiver
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float bombInventory = 0f;


    private Vector2 movement;

    void Start()
    {

    }

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
}
