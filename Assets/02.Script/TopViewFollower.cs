using UnityEngine;

public class TopViewFollower : MonoBehaviour
{
    public float Distance = 20;
    public GameObject player; // 플레이어 오브젝트를 드래그하여 할당하세요.
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, Distance, player.transform.position.z);
    }

}
