using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum EnemyState
{
    Idle,
    Trace,
    Return,
    Attack,
    Damaged,
    Dead,
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _damagedTime = 0.3f;
    [SerializeField] private float _deathTime = 2f;
    [SerializeField] private float _patrolCoolTime = 3f;
    public EnemyState CurrentState = EnemyState.Idle;
    public float MoveSpeed = 5f;
    public float FindDistance = 10f;
    public float AttackDistance = 2f;
    public float MinDistance = 0.2f;
    public float BulletKnockBackSpeed = 1f;
    public float BombKnockBackSpeed = 10f;
    public int Health = 100;
    public int MaxHealth = 100;
    public int patrolIndex = 0;
    public GameObject CurrentPatrol;
    private float _attackCoolTime = 1f;
    private float _attackTimer = 0f;
    private float _knockBackSpeed;
    private GameObject _player;
    private CharacterController _characterController;
    private Coroutine _idleCoroutine = null;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _characterController = GetComponent<CharacterController>();
        CurrentPatrol = EnemyGenerator.Instance.Spawners[0];
    }

    private void Update()
    {
        switch(CurrentState)
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
        CurrentPatrol = EnemyGenerator.Instance.CurrentSpawner;
        transform.position = CurrentPatrol.transform.position;
        CurrentState = EnemyState.Idle;
        Health = MaxHealth;
    }

    public void TakeDamage(Damage damage)
    {
        if (CurrentState == EnemyState.Dead)
        {
            return;
        }

        Health -= damage.Value;
        Debug.Log("피격! 남은 체력: " + Health);
        if (damage.Type == DamageType.Bullet)
        {
            _knockBackSpeed = BulletKnockBackSpeed;
        }else if(damage.Type == DamageType.Bomb)
        {
            _knockBackSpeed = BombKnockBackSpeed;
        }
        if (Health <= 0)
        {
            CurrentState = EnemyState.Dead;
            Debug.Log($"상태전환: {CurrentState} -> Die");
            CurrentState = EnemyState.Dead;
            StartCoroutine(Die_Coroutine());
            return;
        }


        Debug.Log($"상태전환: {CurrentState} -> Damaged");

        CurrentState = EnemyState.Damaged;
        StartCoroutine(Damaged_Coroutine());
    }
    private IEnumerator Damaged_Coroutine()
    {
        yield return new WaitForSeconds(_damagedTime);
        Debug.Log("상태전환: Damaged -> Trace");
        CurrentState = EnemyState.Trace;
    }
    private IEnumerator Die_Coroutine()
    {
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
         Vector3 dir = _player.transform.position - transform.position;
        dir.y = 0f; // y축 고정
        dir.Normalize();
        _characterController.Move(dir * MoveSpeed * Time.deltaTime);
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

        Vector3 dir = CurrentPatrol.transform.position - transform.position;
        dir.y = 0f; // y축 고정
        dir.Normalize();
        _characterController.Move(dir * MoveSpeed * Time.deltaTime);

    }
    private void Attack()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) > AttackDistance)
        {
            Debug.Log("Attack -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer > _attackCoolTime)
        {
            _attackTimer = 0f;
            Debug.Log("공격!");
            // 공격 로직 추가
        }
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
