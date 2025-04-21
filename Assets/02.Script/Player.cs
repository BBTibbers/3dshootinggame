using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float Stemnia;
    public static Player Instance;
    private void Awake()
    {
        Instance = this;
    }
}
