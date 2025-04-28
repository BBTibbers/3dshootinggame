using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Normal,
    AlwaysChase,
}
public enum EnemyState
{
    Idle,
    Trace,
    Return,
    Attack,
    Damaged,
    Dead,
}

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float _damagedTime = 0.3f;
    [SerializeField] private float _deathTime = 2f;
    [SerializeField] private float _patrolCoolTime = 3f;
    public EnemyType EnemyType = EnemyType.Normal;
    public EnemyState CurrentState = EnemyState.Idle;
    public float MoveSpeed = 5f;
    public float FindDistance = 10f;
    public float AttackDistance = 2f;
    public float MinDistance = 0.2f;
    public float BulletKnockBackSpeed = 1f;
    public float BombKnockBackSpeed = 10f;
    public float SwordKnockBackSpeed = 1f;
    public int Health = 100;
    public int MaxHealth = 100;
    public int patrolIndex = 0;
    public GameObject CurrentPatrol;
    [SerializeField]
    private float _attackCoolTime = 2f;
    private float _attackTimer = 0f;
    private float _knockBackSpeed;
    private GameObject _player;
    private CharacterController _characterController;
    private NavMeshAgent _navMeshAgent;
    private Coroutine _idleCoroutine = null;
    public Action EnemyHealthChnaged;
    private Animator _animator;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = MoveSpeed;
        _player = GameObject.FindGameObjectWithTag("Player");
        _characterController = GetComponent<CharacterController>();
        CurrentPatrol = EnemyGenerator.Instance.Spawners[0];
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (CurrentState == EnemyState.Dead)
            return;

        switch (CurrentState)
        {
            case EnemyState.Idle:
            {
                Idle();
                break;
            }
            case EnemyState.Trace:
            {
                Trace();
                break;
            }
            case EnemyState.Return:
            {
                Return();
                break;
            }
            case EnemyState.Attack:
            {
                Attack();
                break;
            }
            case EnemyState.Damaged:
            {
                Damaged();
                break;
            }
            case EnemyState.Dead:
            {
                Dead();
                break;
            }
        }
    }

    public void Initialize()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        CurrentPatrol = EnemyGenerator.Instance.CurrentSpawner;
        transform.position = new Vector3(CurrentPatrol.transform.position.x + UnityEngine.Random.Range(-1f, 1f),
        CurrentPatrol.transform.position.y, CurrentPatrol.transform.position.z + UnityEngine.Random.Range(-1f, 1f));

        CurrentState = EnemyState.Return;
        Health = MaxHealth;
        EnemyHealthChnaged?.Invoke();

        float randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue < 0.5f)
        {
            EnemyType = EnemyType.Normal;
            FindDistance = 10f;
        }
        else
        {
            EnemyType = EnemyType.AlwaysChase;
            FindDistance = float.PositiveInfinity;
        }
    }

    public void TakeDamage(Damage damage)
    {
        if (CurrentState == EnemyState.Dead)
        {
            return;
        }

        Health -= damage.Value;
        EnemyHealthChnaged?.Invoke();
        Debug.Log("피격! 남은 체력: " + Health);
        if (damage.Type == DamageType.Bullet)
        {
            _knockBackSpeed = BulletKnockBackSpeed;
        }else if(damage.Type == DamageType.Bomb)
        {
            _knockBackSpeed = BombKnockBackSpeed;
        }
        else if (damage.Type == DamageType.Sword)
        {
            _knockBackSpeed = SwordKnockBackSpeed;
        }
        if (Health <= 0)
        {
            Debug.Log($"상태전환: {CurrentState} -> Die");
            CurrentState = EnemyState.Dead;
            StartCoroutine(Die_Coroutine());
            return;
        }


        Debug.Log($"상태전환: {CurrentState} -> Damaged");

        CurrentState = EnemyState.Damaged;
        _animator.ResetTrigger("Hitted");
        _animator.SetTrigger("hitted");
        StartCoroutine(Damaged_Coroutine());
    }
    private IEnumerator Damaged_Coroutine()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        yield return new WaitForSeconds(_damagedTime);
        Debug.Log("상태전환: Damaged -> Trace");
        if (CurrentState != EnemyState.Dead)
            CurrentState = EnemyState.Trace;
    }
    private IEnumerator Die_Coroutine()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        yield return new WaitForSeconds(_deathTime);
        EnemyGenerator.Instance.ReturnEnemy(gameObject);
    }

    private void Idle()
    {
        if(Vector3.Distance(_player.transform.position, transform.position) < FindDistance)
        {
            Debug.Log("Idle -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }
        if(Vector3.Distance(CurrentPatrol.transform.position, transform.position) > MinDistance)
        {
            Debug.Log("Idle -> Return");
            CurrentState = EnemyState.Return;
            return;
        }
        if (_idleCoroutine == null)
        {
            _animator.SetTrigger("idle");
            _idleCoroutine = StartCoroutine(CallChangeSpawner());
        }
    }
    private IEnumerator CallChangeSpawner()
    {
        yield return new WaitForSeconds(_patrolCoolTime);
        ChangePatrol();
        _idleCoroutine = null;
    }
    private void ChangePatrol()
    {
        patrolIndex = (patrolIndex + 1) % EnemyGenerator.Instance.Spawners.Count;
        CurrentPatrol = EnemyGenerator.Instance.Spawners[patrolIndex];
    }

    private void Trace()
    {
        if(_idleCoroutine != null)
        {
            StopCoroutine(_idleCoroutine);
            _idleCoroutine = null;
        }
        if (Vector3.Distance(_player.transform.position, transform.position) > FindDistance)
        {
            Debug.Log("Trace -> Return");
            CurrentState = EnemyState.Return;
            return;
        }
        if (Vector3.Distance(_player.transform.position, transform.position) < AttackDistance)
        {
            Debug.Log("Trace -> Attack");
            CurrentState = EnemyState.Attack;
            return;
        }
        _animator.SetTrigger("walk");
         Vector3 dir = _player.transform.position - transform.position;
        dir.y = 0f; // y축 고정
        dir.Normalize();
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _navMeshAgent.SetDestination(_player.transform.position);
    }
    private void Return()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) < FindDistance)
        {
            Debug.Log("Return -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }
        if(Vector3.Distance(CurrentPatrol.transform.position, transform.position) < MinDistance)
        {
            transform.position = CurrentPatrol.transform.position;
            Debug.Log("Return -> Idle");
            CurrentState = EnemyState.Idle;
            return;
        }

        _animator.SetTrigger("walk");
        Vector3 dir = CurrentPatrol.transform.position - transform.position;
        dir.y = 0f; // y축 고정
        dir.Normalize();
        //_characterController.Move(dir * MoveSpeed * Time.deltaTime);
        _navMeshAgent.SetDestination(CurrentPatrol.transform.position);

    }
    public void Attack()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        if (Vector3.Distance(_player.transform.position, transform.position) > AttackDistance)
        {
            Debug.Log("Attack -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }
        _animator.SetTrigger("attackDelay");
        _attackTimer += Time.deltaTime;
        if (_attackTimer > _attackCoolTime)
        {
            _animator.SetTrigger("attack");
            _attackTimer = 0f;
        }
    }
    public void Hit()
    {
        // 공격 로직 추가
        _player.GetComponent<Player>().TakeDamage(new Damage { Value = 10, Type = DamageType.Enemy, From = gameObject });
    }
    private void Damaged()
    {
        KnockBack();
    }
    private void KnockBack()
    {
        Vector3 dir = transform.position - _player.transform.position;
        dir.y = 0f; // y축 고정
        dir.Normalize();
        _characterController.Move(dir * _knockBackSpeed * Time.deltaTime);
        Debug.Log("KnockBack");
    }
    private void Dead()
    {
    }
}
