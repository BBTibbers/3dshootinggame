using UnityEngine;

public class Coin : MonoBehaviour
{
    private float _rotationSpeed = 300f;
    private float _moveSpeed = 5f;
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
    }

    private void Follow()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < 10f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, _moveSpeed * Time.deltaTime);
        }
        if(Vector3.Distance(transform.position, player.transform.position) < 1f)
        {
            Destroy(gameObject);
        }
    }
}
