using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float rotationSpeed = 125f;
    private float _rotationX;
    private float _rotationY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _rotationX += mouseX * rotationSpeed * Time.deltaTime;
        _rotationY -= mouseY * rotationSpeed * Time.deltaTime;

        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);
        transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0);
    }
}
