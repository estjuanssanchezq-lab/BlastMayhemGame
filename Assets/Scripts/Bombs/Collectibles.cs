using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [SerializeField] private PickupType pickupType;
    [SerializeField] private float lifeTime = 6f; // Tiempo de duración antes de que el objeto se destruya automáticamente

    void Start()
    {
        Destroy(gameObject, lifeTime); // Destruye el objeto después de lifeTime segundos
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Lógica para agregar la bomba al inventario del jugador...
            // Basicamente esta linea dice: dame el script PlayerTest que está
            // pegado a este GameObjecty. 

            // player por su parte guarda la referencia al script, lo que permite
            // acceder a sus método y variables. En este caso, el método AddBomb()

            // Tipo de dato PlayerTest, nombre de variable player

            IPlayerPickupReceiver player = collision.gameObject.GetComponent<IPlayerPickupReceiver>();

            switch (pickupType)
            {
                case PickupType.Bomb:
                    Debug.Log("Bomba recogida");
                    player.AddBomb();
                    break;

                case PickupType.Health:
                    Debug.Log("Vida recogida");
                    break;

                case PickupType.Boomerang:
                    Debug.Log("Boomerang recogido");
                    break;
            }

            Destroy(gameObject);
        }
    }



}
