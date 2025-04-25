using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BombPool : MonoBehaviour
{
    public static BombPool Instance; // 싱글톤 인스턴스

    [SerializeField] private GameObject _bombPrefab; // 폭탄 프리팹
    [SerializeField] private int PoolSize = 3;
    private List<GameObject> _bombPool;
    public GameObject Player; // 플레이어 오브젝트

    private void Awake()
    {
        Instance = this;
        _bombPool = new List<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject bomb = Instantiate(_bombPrefab,this.transform);
            bomb.SetActive(false);
            _bombPool.Add(bomb);
        }
    }

    public GameObject GetBomb()
    {
        foreach (var bomb in _bombPool)
        {
            if (!bomb.activeSelf)
            {
                bomb.SetActive(true);
                Physics.IgnoreCollision(bomb.GetComponent<Collider>(), Player.GetComponent<Collider>());
                return bomb;
            }
        }
        // 풀에 사용 가능한 폭탄이 없으면 새로 생성
        GameObject newBomb = Instantiate(_bombPrefab);
        newBomb.SetActive(true);
        _bombPool.Add(newBomb);
        return newBomb;
    }

    public void ReturnBomb(GameObject bomb)
    {
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        bomb.SetActive(false);
    }

}
