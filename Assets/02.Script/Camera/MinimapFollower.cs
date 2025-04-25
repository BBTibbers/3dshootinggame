using UnityEngine;

public class MinimapFollower : MonoBehaviour
{
    public float Distance = 100f;
    public GameObject player; // 플레이어 오브젝트를 드래그하여 할당하세요.
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, Distance, player.transform.position.z);
    }

    public void DistancePlus()
    {
        this.GetComponent<Camera>().orthographicSize = 10f;
    }

    public void DistanceMinus()
    {
        this.GetComponent<Camera>().orthographicSize = 5f;
    }
}
