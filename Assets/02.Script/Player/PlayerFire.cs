using System;
using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class PlayerFire : MonoBehaviour
{

    [SerializeField] private float _fireCooltime = 0.1f;
    [SerializeField] private int _maxBullets = 50;
    [SerializeField] private float _reloadingTime = 2f;
    [SerializeField] private LineRenderer tracerLine;
    [SerializeField] private float tracerDuration = 0.01f;
    [SerializeField] private int _bulletDamage = 10;
    [SerializeField] private ParticleSystem muzzleFlash;
    public GameObject FirePosition;
    public GameObject BombPrefab;
    public GameObject Player;
    public GameObject BulletEffect;
    public GameObject FireEffect;
    public Action<int, int> BulletCountChange;
    public Action OneShot;
    private float _nextFire = 0;
    private int _remainBullets = 50;
    private bool _reloading = false;
    private float _nextReloadingTime = 0;
    public GameObject UI_CrossHair;
    public GameObject UI_SniperZoom;
    private bool _zoomMode = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        BulletCountChange?.Invoke(_remainBullets, _maxBullets);
        muzzleFlash.Stop(); // (재생 중이라도 강제로 끊고)

    }
    private void Update()
    {
        Fire();
        if (Input.GetKeyDown(KeyCode.R))
        {
            _reloading = true;
        }
        ReLoad();
        Zoom();
    }

    private void Zoom()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _zoomMode = !_zoomMode;
            UI_CrossHair.SetActive(!_zoomMode);
            UI_SniperZoom.SetActive(_zoomMode);
            if (_zoomMode)
            {
                Camera camera = CameraController.Instance.GetActiveCamera();
                camera.fieldOfView = 10f;
                CameraController.Instance.RotationSpeed *= 0.1f;
            }
            else
            {
                Camera camera = CameraController.Instance.GetActiveCamera();
                camera.fieldOfView = 60f;
                CameraController.Instance.RotationSpeed *= 10f;
            }
        }
    }

    private void Fire()
    {
        if (muzzleFlash == null)
        {
            Debug.LogError("muzzleFlash 레퍼런스가 null이다! (오브젝트가 사라졌거나 연결이 끊겼음)");
        }
        else
        {
            Debug.Log("muzzleFlash는 살아있음");
        }
        if (Input.GetMouseButtonDown(0))
            _reloading = false;
        if (Input.GetMouseButton(0))
        {
            if (Time.time < _nextFire)
                return;
            if (_remainBullets == 0)
            {
                _reloading = true;
                return;
            }
            if (_reloading)
                return;

            _remainBullets--;
            BulletCountChange?.Invoke(_remainBullets, _maxBullets);
            _nextFire = Time.time + _fireCooltime;
            //GameObject fireVFX = Instantiate(FireEffect);
            //fireVFX.transform.position = FirePosition.transform.position;
            //fireVFX.GetComponent<ParticleSystem>().Play();
            //Destroy(fireVFX,_fireCooltime);
            muzzleFlash.Play(); // 새로 한 번 재생
            Camera camera = CameraController.Instance.GetActiveCamera();
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            RaycastHit hitInfo = new RaycastHit();
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit)
            {
                GameObject vfx = Instantiate(BulletEffect);
                vfx.transform.position = hitInfo.point;
                vfx.GetComponent<ParticleSystem>().Play();
                StartCoroutine(ShowTracer(FirePosition.transform.position, hitInfo.point));
                Destroy(vfx, 2f);

                IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Damage damage = new Damage();
                    damage.Value = _bulletDamage;
                    damage.From = this.gameObject;
                    damage.Type = DamageType.Bullet;
                    damageable.TakeDamage(damage);
                }
            }
            OneShot?.Invoke();

        }
    }
    IEnumerator ShowTracer(Vector3 start, Vector3 end)
    {
        tracerLine.positionCount = 2; // 꼭 먼저 설정해줘야 함
        tracerLine.SetPosition(0, start);
        tracerLine.SetPosition(1, end);
        tracerLine.gameObject.SetActive(true);
        yield return new WaitForSeconds(tracerDuration);

        tracerLine.gameObject.SetActive(false);
    }

    private void ReLoad()
    {
        if (!_reloading)
            return;
        if (_remainBullets == _maxBullets)
        {
            _reloading = false;
            return;
        }
        if (_nextReloadingTime > Time.time)
        {
            return;
        }
        _nextReloadingTime = Time.time + (_reloadingTime / _maxBullets);
        _remainBullets++;
        BulletCountChange?.Invoke(_remainBullets, _maxBullets);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Player.transform.position, Player.transform.forward * 5f);
    }

}
