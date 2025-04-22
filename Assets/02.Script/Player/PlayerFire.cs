using System;
using UnityEngine;
using System.Collections;

public class PlayerFire : MonoBehaviour
{

    [SerializeField] private float _throwPower = 15;
    [SerializeField] private int _bombCount = 3;
    [SerializeField] private int _maxBombCount = 3;
    [SerializeField] private float _maxChargeTime = 3f;
    [SerializeField] private float _fireCooltime = 0.1f;
    [SerializeField] private int _maxBullets = 50;
    [SerializeField] private float _reloadingTime = 2f;
    public GameObject FirePosition;
    public GameObject BombPrefab;
    public GameObject Player;
    public GameObject BulletEffect;
    public Action<int, int> BombCountChange;
    public Action <int, int> BulletCountChange;
    public Action OneShot;
    private bool _bombCharging = false;
    private float _chargeTime;
    private float _nextFire = 0;
    private int _remainBullets = 50;
    private bool _reloading = false;
    private float _nextReloadingTime = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        BombCountChange?.Invoke(_bombCount, _maxBombCount);
        BulletCountChange?.Invoke(_remainBullets, _maxBullets);
    }
    private void Update()
    {
        ThrowBomb();
        Fire();
        if(Input.GetKeyDown(KeyCode.R))
        {
            _reloading = true;
        }
        ReLoad();
    }

    private void Fire()
    {
        if(Input.GetMouseButtonDown(0))
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
            if(_reloading)
                return;
            
            _remainBullets--;
            BulletCountChange?.Invoke(_remainBullets, _maxBullets);
            _nextFire = Time.time + _fireCooltime;
            Ray ray = new Ray(Player.transform.position, Player.transform.forward);
            RaycastHit hitInfo = new RaycastHit();
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit)
            {
                GameObject vfx = Instantiate(BulletEffect);
                vfx.transform.position = hitInfo.point;
                vfx.GetComponent<ParticleSystem>().Play();
                Destroy(vfx, 2f);
            }
            OneShot?.Invoke();

        }
    }

    private void ReLoad()
    {
        if (!_reloading)
            return;
        if(_remainBullets == _maxBullets)
        {
            _reloading = false;
            return;
        }
        if(_nextReloadingTime>Time.time)
        {
            return;
        }
        _nextReloadingTime = Time.time + (_reloadingTime / _maxBullets);
        _remainBullets++;
        BulletCountChange?.Invoke(_remainBullets, _maxBullets);
    }


    private void ThrowBomb()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_bombCount <= 0)
            {
                return;
            }
            _bombCharging = true;
            _chargeTime = Time.time;
            UIManager.Instance.BombChargeShow(_maxChargeTime);
        }
        if (Input.GetMouseButtonUp(1) && _bombCharging)
        {
            UIManager.Instance.BombChargeHide();
            _bombCount--;
            BombCountChange?.Invoke(_bombCount, _maxBombCount);
            GameObject bomb = BombPool.Instance.GetBomb();
            bomb.transform.position = FirePosition.transform.position;

            Rigidbody bombRIgidbody = bomb.GetComponent<Rigidbody>();
            float charged = Mathf.Min(Time.time - _chargeTime, _maxChargeTime);
            bombRIgidbody.AddForce(Player.transform.forward * _throwPower * charged, ForceMode.Impulse);
            bombRIgidbody.AddTorque(Vector3.one);

            _bombCharging = false;
        }
    }
}
