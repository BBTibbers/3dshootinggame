using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private float _explosionForce = 5f;  // 폭발 강도
    [SerializeField] private float _explosionRadius = 5f;   // 폭발 반경
    [SerializeField] private float _upwardsModifier = 0f;   // 위로 밀어내는 힘
    [SerializeField] private int _explosionDamage = 110; // 폭발 피해량
    [SerializeField] private float _explsionPositionPreset = 3;
    [SerializeField] private float _explosionTouque = 10f; // 폭발 회전력

    public int Health = 200;
    [SerializeField] private GameObject _vfx;
    public void TakeDamage(Damage damage)
    {
        if(Health>0)
            Health -= damage.Value;
        if (Health <= 0)
            Explosion();
    }
    private void Explosion()
    {
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
            rb.AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * _explosionTouque, ForceMode.Impulse); // 회전 추가
            Debug.Log("Barrel Exploded");
        }

        // 주변 적들에게 피해를 주는 함수
        DetectEnemies();

        // 바렐 객체를 파괴
        Destroy(gameObject,5f);
    }
    void DetectEnemies()
    {
        Vector3 center = transform.position;

        Collider[] hitColliders = Physics.OverlapSphere(center, _explosionRadius);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                Debug.Log("Barrel Hit Enemy");

                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Damage damage = new Damage
                    {
                        Value = _explosionDamage,
                        From = this.gameObject,
                        Type = DamageType.Bomb
                    };
                    enemy.TakeDamage(damage);
                }
            }
            if(col.CompareTag("Player"))
            {
                Debug.Log("Barrel Hit Player");
                Player player = col.GetComponent<Player>();
                if (player != null)
                {
                    Damage damage = new Damage
                    {
                        Value = _explosionDamage,
                        From = this.gameObject,
                        Type = DamageType.Bomb
                    };
                    player.TakeDamage(damage);
                }
            }
        }
    }
}
