using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float WalkSpeed = 7f;
    public float RunSpeed = 12;
    public float SprintSpeed = 20f;
    public float ClimbSpeed = 2f;
    private float _speed;
    private CharacterController _characterController;
    private const float GRAVITY = -9.8f;
    private float _yVelocity = 0f;
    public float JumpPower = 2f;
    public float SteminaMax = 10f;
    private float _stemnia;
    private float _exhaustion = 0f;
    public Slider SteminaSlider;
    private float jumpCooldown = 0.1f;
    private float jumpTimer = 0f;
    private Vector3 _sprintDir = new Vector3(0, 0, 1);
    private int _isJumping = 0;
    public Collider ClimbCollider;
    public bool IsClimbing = false;
    private float _h;
    private float _v;
    private float SprintTime = 0f;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _stemnia = SteminaMax;
    }

    // Update is called once per frame
    private void Update()
    {
        _h = Input.GetAxis("Horizontal");
        _v = Input.GetAxis("Vertical");

        if (IsClimbing && !_characterController.isGrounded && _stemnia>0) // 벽타기 조건이면 조작이 바뀜
        {
            Climbing();
            return;
        }

        Vector3 dir = new Vector3(_h, 0, _v).normalized;

        if (Time.time<SprintTime || (Input.GetKeyDown(KeyCode.E)&&_stemnia>1)) // 대시(구르기)
        {
            Sprint(dir);
            return;
        }

        Jump();
        Run();
        
        _yVelocity += GRAVITY * Time.deltaTime; 
        dir = Camera.main.transform.TransformDirection(dir);
        dir.y = _yVelocity;

        SteminaSlider.value = _stemnia / SteminaMax;

        _characterController.Move( dir * _speed * Time.deltaTime);
    }
    private void Sprint(Vector3 dir)
    {
        _stemnia -= 5 * Time.deltaTime;

        if (SprintTime < Time.time)
        {
            //_sprintDir = Vector3.forward; 방향고정 - 앞으로만 대시
            _sprintDir = dir; // 방향고정 - 조작감이 더 나음
            _sprintDir = Camera.main.transform.TransformDirection(_sprintDir);
            _sprintDir.y = 0f; // 대시 중 고도 고정
            _sprintDir.Normalize();
            SprintTime = Time.time + 0.2f;//0.2초간 대시
        }

        _characterController.Move(_sprintDir * SprintSpeed * Time.deltaTime);
        SteminaSlider.value = _stemnia / SteminaMax;
    }
    private void Jump()
    {
        jumpTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && _isJumping < 2)
        {
            _yVelocity = JumpPower;
            ++_isJumping;
            jumpTimer = jumpCooldown; // 잠시 grounded 체크 막기
        }
        else if (jumpTimer <= 0f && _characterController.isGrounded)
        {
            _isJumping = 0;
        }
    }
    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && (_h != 0 || _v != 0) && _stemnia > 0) // 움직일 때만 달리기 판정
        {
            _speed = RunSpeed;
            _stemnia -= Time.deltaTime;
        }
        else
        {
            _speed = WalkSpeed;
            if (_stemnia < SteminaMax && Time.time > _exhaustion)
            {
                _stemnia += Time.deltaTime;
            }
        }
        if (_stemnia <= 0 && Time.time > _exhaustion) // 스테미나 0되면 잠시 탈진
        {
            _exhaustion = Time.time + 3f; // 탈진시 3초간 스테미너 회복 불가
        }
    }

    private void Climbing()
    {
        _yVelocity = 0f;
        Vector3 dir = new Vector3(_h, _v, 0).normalized; // ws로 고도를 조정하도록 조작키 변경
        dir = Camera.main.transform.TransformDirection(dir);
        _characterController.Move(dir * ClimbSpeed * Time.deltaTime);
        _stemnia -= 0.5f * Time.deltaTime;
        SteminaSlider.value = _stemnia / SteminaMax;

        if (_stemnia <= 0 && Time.time > _exhaustion)
        {
            _exhaustion = Time.time + 3f; // 탈진시 3초간 대기

        }
    }
}
