using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float SteminaMax = 10f;
    public float Stemnia;
    public static Player Instance;
    private void Awake()
    {
        Instance = this;
    }
}
