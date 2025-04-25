using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private float fixRecoilTime = 2f;
    [SerializeField] private float maxRecoil = 2f;
    [SerializeField] private float _rotationSpeed = 1250f;
    [SerializeField] private float _recoilY = 0.5f;
    public GameObject Cam1;
    public GameObject Cam2;
    public GameObject Cam3;
    private float _rotationX;
    private float _rotationY;
    private float _currentRecoilY = 0f;

    void SwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Cam1.SetActive(true);
            Cam2.SetActive(false);
            Cam3.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Cam1.SetActive(false);
            Cam2.SetActive(true);
            Cam3.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Cam1.SetActive(false);
            Cam2.SetActive(false);
            Cam3.SetActive(true);
        }
    }
    private void Awake()
    {
        Instance = this;
        GameObject[] guns = GameObject.FindGameObjectsWithTag("Gun");

        foreach (GameObject gun in guns)
        {
            PlayerFire fire = gun.GetComponent<PlayerFire>();
            if (fire != null)
            {
                fire.OneShot += () =>
                {
                    _currentRecoilY = Mathf.Min(
                        _currentRecoilY + _recoilY * (maxRecoil - _currentRecoilY) / maxRecoil,
                        maxRecoil
                    );
                };
            }
        }

    }
    void Update()
    {

        if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }                  
        Recoil();
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _rotationX += mouseX * _rotationSpeed * Time.deltaTime;
        _rotationY -= mouseY * _rotationSpeed * Time.deltaTime;
  

        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);
        transform.eulerAngles = new Vector3(_rotationY-_currentRecoilY, _rotationX, 0);
        SwitchCamera();
        
    }
    private void Recoil()
    {
        if (GetActiveCamera() == Cam3)
            return;
        _currentRecoilY -= maxRecoil * Time.deltaTime / fixRecoilTime;
        _currentRecoilY = Mathf.Max(_currentRecoilY, 0);
    }

    public Camera GetActiveCamera()
    {
        Camera[] allCameras = Camera.allCameras;
        foreach (Camera cam in allCameras)
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy && cam.CompareTag("MainCamera"))
            {
                return cam;
            }
        }
        return null;
    }

}
