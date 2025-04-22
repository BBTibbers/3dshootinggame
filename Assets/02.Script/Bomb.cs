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
    }
}
