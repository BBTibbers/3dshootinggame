using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public Collider ClimbCollider;
    public PlayerSO PlayerSO;
    public Action SteminaChanged;
    public bool IsClimbing = false;
    private int _isJumping = 0;
    private float _speed;
    private float _yVelocity = 0f;
    private float _exhaustion = 0f;
    private float _jumpTimer = 0f;
    private float _h;
    private float _v;
    private float _sprintTime = 0f;
    private Vector3 _sprintDir = new Vector3(0, 0, 1);
    private CharacterController _characterController;
    private const float GRAVITY = -9.8f;
    private const float JUMP_COOLDOWN = 0.1f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Player.Instance.Stemina = PlayerSO.SteminaMax;
    }

    // Update is called once per frame
    private void Update()
    {
        _h = Input.GetAxis("Horizontal");
        _v = Input.GetAxis("Vertical");

        if (IsClimbing && !_characterController.isGrounded && Player.Instance.Stemina>0) // 벽타기 조건이면 조작이 바뀜
        {
            Climbing();
            return;
        }

        Vector3 dir = new Vector3(_h, 0, _v).normalized;

        if (Time.time<_sprintTime || (Input.GetKeyDown(KeyCode.E)&&Player.Instance.Stemina>1)) // 대시(구르기)
        {
            Sprint(dir);
            return;
        }

        Jump();
        Run();
        
        _yVelocity += GRAVITY * Time.deltaTime; 
        dir = transform.TransformDirection(dir);
        dir.y = _yVelocity;
        SteminaChanged?.Invoke();
        _characterController.Move( dir * _speed * Time.deltaTime);
    }
    private void Sprint(Vector3 dir)
    {
        Player.Instance.Stemina -= 5 * Time.deltaTime;

        if (_sprintTime < Time.time)
        {
            //_sprintDir = Vector3.forward; 방향고정 - 앞으로만 대시
            _sprintDir = dir; // 방향고정 - 조작감이 더 나음
            _sprintDir = transform.TransformDirection(_sprintDir);
            _sprintDir.y = 0f; // 대시 중 고도 고정
            _sprintDir.Normalize();
            _sprintTime = Time.time + PlayerSO.DashTime;//0.2초간 대시
        }

        _characterController.Move(_sprintDir * PlayerSO.SprintSpeed * Time.deltaTime);
        SteminaChanged?.Invoke();
    }
    private void Jump()
    {
        _jumpTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && _isJumping < PlayerSO.JumpCount)
        {
            _yVelocity = PlayerSO.JumpPower;
            ++_isJumping;
            _jumpTimer = JUMP_COOLDOWN; // 잠시 grounded 체크 막기
        }
        else if (_jumpTimer <= 0f && _characterController.isGrounded)
        {
            _isJumping = 0;
        }
    }
    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && (_h != 0 || _v != 0) && Player.Instance.Stemina > 0) // 움직일 때만 달리기 판정
        {
            _speed = PlayerSO.RunSpeed;
            Player.Instance.Stemina -= Time.deltaTime;
        }
        else
        {
            _speed = PlayerSO.WalkSpeed;
            if (Player.Instance.Stemina < PlayerSO.SteminaMax && Time.time > _exhaustion)
            {
                Player.Instance.Stemina += Time.deltaTime;
            }
        }
        if (Player.Instance.Stemina <= 0 && Time.time > _exhaustion) // 스테미나 0되면 잠시 탈진
        {
            _exhaustion = Time.time + 3f; // 탈진시 3초간 스테미너 회복 불가
        }
    }

    private void Climbing()
    {
        _yVelocity = 0f;
        Vector3 dir = new Vector3(_h, _v, 0).normalized; // ws로 고도를 조정하도록 조작키 변경
        dir = transform.TransformDirection(dir);
        _characterController.Move(dir * PlayerSO.ClimbSpeed * Time.deltaTime);
        Player.Instance.Stemina -= 0.5f * Time.deltaTime;
        SteminaChanged?.Invoke();

        if (Player.Instance.Stemina <= 0 && Time.time > _exhaustion)
        {
            _exhaustion = Time.time + 3f; // 탈진시 3초간 대기

        }
    }
}
