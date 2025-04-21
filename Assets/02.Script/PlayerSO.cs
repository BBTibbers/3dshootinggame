using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "ScriptableObjects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public float WalkSpeed = 7f;
    public float RunSpeed = 12;
    public float SprintSpeed = 20f;
    public float ClimbSpeed = 2f;
    public float JumpPower = 2f;
    public int JumpCount = 2; // 점프 횟수
    public float DashTime = 0.2f; // 대시 시간
}
