using UnityEditor.PackageManager;
using UnityEngine;

public class Barrel : MonoBehaviour , IDamageable
{
    [SerializeField] private float _explosionForce = 5f;  // 폭발 강도
    [SerializeField] private float _explosionRadius = 5f;   // 폭발 반경
    [SerializeField] private float _upwardsModifier = 0f;   // 위로 밀어내는 힘
    [SerializeField] private int _explosionDamage = 110; // 폭발 피해량
    [SerializeField] private float _explsionPositionPreset = 2;
    [SerializeField] private float _explosionTouque = 10f; // 폭발 회전
    [SerializeField] private GameObject _vfx;
    private bool _isExploded = false; // 폭발 여부 
    public int Health = 200;

    public void TakeDamage(Damage damage)
    {
        if(Health>0)
            Health -= damage.Value;
        if (Health <= 0)
            Explosion();
    }
    private void Explosion()
    {
        if(_isExploded)
            return;
        _isExploded = true; // 폭발 상태로 변경
        // 폭발 효과를 나타내는 VFX
        GameObject vfx = Instantiate(_vfx);
        vfx.transform.position = transform.position;
        vfx.transform.localScale = new Vector3(3f, 3f, 3f);

        // 자기 자신을 폭발로 날리기
        Rigidbody rb = GetComponent<Rigidbody>();  // Rigidbody 가져오기
        if (rb != null)
        {
            Vector3 explosionPosition = transform.position; // 폭발 위치   
            explosionPosition.y = 0f;
            explosionPosition.x += Random.Range(-_explsionPositionPreset, _explsionPositionPreset);
            explosionPosition.y += Random.Range(-_explsionPositionPreset, _explsionPositionPreset);
            // 자기 자신을 날리기 위한 폭발 힘 적용
            rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _upwardsModifier, ForceMode.Impulse);
            rb.AddTorque(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * _explosionTouque, ForceMode.Impulse); // 회전 추가
            Debug.Log("Barrel Exploded");
        }

        // 주변 적들에게 피해를 주는 함수
        DetectEnemies();

        // 바렐 객체를 파괴
        Destroy(gameObject,5f);
    }
    private void DetectEnemies()
    {
        Vector3 center = transform.position;

        Collider[] hitColliders = Physics.OverlapSphere(center, _explosionRadius);
        foreach (Collider col in hitColliders)
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damage damage = new Damage();
                damage.Value = _explosionDamage;
                damage.From = this.gameObject;
                damage.Type = DamageType.Barrel;
                damageable.TakeDamage(damage);
            }
        }
    }
}
