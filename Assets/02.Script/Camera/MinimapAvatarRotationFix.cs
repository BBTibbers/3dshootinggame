using UnityEngine;

public class MinimapAvatarRotationFix : MonoBehaviour
{
    private Quaternion fixedWorldRotation;

    void Start()
    {
        // 미니맵에서 위를 보는 회전
        fixedWorldRotation = Quaternion.Euler(90f, 0f, 0f);
    }

    void LateUpdate()
    {
        transform.rotation = fixedWorldRotation;
    }
}
