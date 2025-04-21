using UnityEngine;

public class ClimbTrigger : MonoBehaviour
{
    public PlayerMove PlayerMove;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // 자기자신충돌 방지
            return;
        PlayerMove.IsClimbing = true;
    }
    public void OnTriggerExit(Collider other)
    {
        PlayerMove.IsClimbing = false;
    }
}
