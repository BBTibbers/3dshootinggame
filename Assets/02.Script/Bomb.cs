using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private GameObject _vfx;   

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        GameObject vfx = Instantiate(_vfx);
        vfx.transform.position = transform.position;
        BombPool.Instance.ReturnBomb(this.gameObject);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Damage damage = new Damage();
            damage.Value = 200;
            damage.From = this.gameObject;
            damage.Type = DamageType.Bomb;
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}
