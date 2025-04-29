using System;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private bool _bombCharging = false;
    private float _chargeTime;

    [SerializeField] private int _bombCount = 3;
    [SerializeField] private int _maxBombCount = 3;
    [SerializeField] private float _maxChargeTime = 3f;
    public Action<int, int> BombCountChange;
    [SerializeField] private float _throwPower = 15;
    // Update is called once per frame

    private void Start()
    {
        BombCountChange?.Invoke(_bombCount, _maxBombCount);
    }
    void Update()
    {
        ThrowBomb();
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
            PlayerUI.Instance.BombChargeShow(_maxChargeTime);
        }
        if (Input.GetMouseButtonUp(1) && _bombCharging)
        {
            PlayerUI.Instance.BombChargeHide();
            _bombCount--;
            BombCountChange?.Invoke(_bombCount, _maxBombCount);
            GameObject bomb = BombPool.Instance.GetBomb();
            bomb.transform.position = transform.position;

            Rigidbody bombRIgidbody = bomb.GetComponent<Rigidbody>();
            float charged = Mathf.Min(Time.time - _chargeTime, _maxChargeTime);
            bombRIgidbody.AddForce(transform.forward.normalized * _throwPower * charged, ForceMode.Impulse);
            bombRIgidbody.AddTorque(Vector3.one);

            _bombCharging = false;
        }
    }
}
