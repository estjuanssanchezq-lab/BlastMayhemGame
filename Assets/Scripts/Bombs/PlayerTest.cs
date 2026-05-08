using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [SerializeField, Min(0)] private int bombInventory = 0;

    public int BombInventory => bombInventory;

    public void AddBomb()
    {
        bombInventory++;
        Debug.Log("Bombas en inventario: " + bombInventory);
    }
}
