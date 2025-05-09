using UnityEngine;
using DG.Tweening;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private GameObject _vfx;
    [SerializeField] private float _explosionRadius = 5f;   // 폭발 반경
    [SerializeField] private int _explosionDamage = 110; // 폭발 피해량

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        GameObject vfx = Instantiate(_vfx);
        vfx.transform.position = transform.position;
        BombPool.Instance.ReturnBomb(this.gameObject);

        CameraController.Instance.GetActiveCamera().DOShakePosition(0.5f, 0.5f, 10, 90, true);

        DetectEnemies();

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
                damage.Type = DamageType.Bomb;
                damageable.TakeDamage(damage);
            }
        }
    }
}
