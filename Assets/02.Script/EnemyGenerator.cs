using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator Instance; // 싱글톤 인스턴스
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] int PoolSize = 3;
    private List<GameObject> _enemyPool;
    public GameObject CurrentSpawner;
    public List<GameObject> Spawners = new List<GameObject>();
    private Coroutine _isGenerating;
    private void Awake()
    {
        Instance = this;
        _enemyPool = new List<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject enemy = Instantiate(_enemyPrefab, this.transform);
            enemy.SetActive(false);
            _enemyPool.Add(enemy);
        }
        CurrentSpawner = Spawners[0];
    }

    private void Update()
    {
        if(_isGenerating == null)
        {
            _isGenerating = StartCoroutine(GenerateEnemy());
        }

    }
    private IEnumerator GenerateEnemy()
    {
        yield return new WaitForSeconds(5f);
        GameObject enemy = GetEnemy();
        if (enemy != null)
        {
            int patrolIndex =  Random.Range(0, Spawners.Count);
            enemy.GetComponent<Enemy>().patrolIndex = patrolIndex;
            CurrentSpawner = Spawners[patrolIndex];
            enemy.GetComponent<Enemy>().Initialize();
            enemy.SetActive(true);
        }
        _isGenerating = null;
    }

    public GameObject GetEnemy()
    {
        foreach (var enemy in _enemyPool)
        {
            if (!enemy.activeSelf)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }
        return null;
    }


    public void ReturnEnemy(GameObject bomb)
    {
        bomb.SetActive(false);
    }
}
