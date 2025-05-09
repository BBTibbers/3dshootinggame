using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _attackAngle = 60f; // 부채꼴 각도
    [SerializeField] private int _swordDamage = 20;
    [SerializeField] private GameObject _swordPosition;
    [SerializeField] private float _sowrdCooltime = 1f;
    private float _nextSwordtime = 0;
    private bool _attacking = false;
    [SerializeField] private GameObject _swordEffectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if (_attacking)
            return;
        if (_nextSwordtime > Time.time)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            _attacking = true;
            _nextSwordtime = Time.time+_sowrdCooltime;

            Quaternion baseRotation = Quaternion.LookRotation(-_swordPosition.transform.forward);
            Quaternion finalRotation = baseRotation * Quaternion.Euler(45f, 0f, 135f); // ← Z축 회전 추가

            GameObject vfx = Instantiate(_swordEffectPrefab, _swordPosition.transform.position, finalRotation);
            vfx.transform.localScale *= 1f; // 또는 new Vector3(2f, 2f, 2f);

            Destroy(vfx, 1f);
            
            Collider[] hitEnemies = Physics.OverlapSphere(_swordPosition.transform.position, _attackRange);

            foreach (Collider col in hitEnemies)
            {
                Vector3 dirToTarget = (col.ClosestPoint(_swordPosition.transform.position) - _swordPosition.transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);
                //angle < _attackAngle * 0.5f
                if (angle < _attackAngle * 0.5f)
                {
                    IDamageable damageable = col.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        Damage damage = new Damage();
                        damage.Value = _swordDamage;
                        damage.From = this.gameObject;
                        damage.Type = DamageType.Sword;
                        damageable.TakeDamage(damage);
                    }
                }
            }
            _attacking = false;
        }
    }
}
