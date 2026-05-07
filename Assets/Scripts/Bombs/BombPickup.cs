using UnityEngine;

public class BombPickup : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bomba recogida");

            // Lógica para agregar la bomba al inventario del jugador...
            // Basicamente esta linea dice: dame el script PlayerTest que está
            // pegado a este GameObjecty. 

            // player por su parte guarda la referencia al script, lo que permite
            // acceder a sus método y variables. En este caso, el método AddBomb()

            // Tipo de dato PlayerTest, nombre de variable player
            PlayerTest player = collision.gameObject.GetComponent<PlayerTest>();
            player.AddBomb();

            Destroy(gameObject);
        }
    }
}