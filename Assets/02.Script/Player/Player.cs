using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float Stemina;
    public int Health = 100;
    public PlayerSO PlayerSO;
    public static Player Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void TakeDamage(Damage damage)
    {
        Debug.Log("Player Hitted");
    }
}
