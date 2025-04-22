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
    private float _rotationX;
    private float _rotationY;
    private float _currentRecoilY = 0f;

    void SwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Cam1.SetActive(true);
            Cam2.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Cam1.SetActive(false);
            Cam2.SetActive(true);
        }
    }
    private void Awake()
    {
        Instance = this;
        Player.Instance.GetComponent<PlayerFire>().OneShot += () =>
        {
            _currentRecoilY = Mathf.Min(_currentRecoilY + _recoilY *(maxRecoil-_currentRecoilY)/maxRecoil, maxRecoil);
        };
    }
    void Update()
    {

        Cursor.lockState = CursorLockMode.Locked;  // 마우스를 화면 중앙에 고정
        Cursor.visible = false;                    // 커서를 보이지 않게
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
        _currentRecoilY -= maxRecoil * Time.deltaTime / fixRecoilTime;
        _currentRecoilY = Mathf.Max(_currentRecoilY, 0);
    }
}
