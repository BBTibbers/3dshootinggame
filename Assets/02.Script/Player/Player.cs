using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float Stemina;
    public PlayerSO PlayerSO;
    public static Player Instance;
    private void Awake()
    {
        Instance = this;
    }
}
