using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Update()
    {
        Camera camera = CameraController.Instance.GetActiveCamera();
        transform.forward = camera.transform.forward;
        //transform.Rotate(0, 180, 0); // 필요하면 뒤집기 (이미지 방향에 따라)
    }
}

