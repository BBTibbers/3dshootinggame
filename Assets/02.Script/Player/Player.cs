using UnityEngine;
using System;

public class Player : MonoBehaviour, IDamageable
{

    public float Stemina;
    public int Health = 500;
    public PlayerSO PlayerSO;
    public static Player Instance;
    public Action PlayerHealthChanged;
    private void Awake()
    {
        Instance = this;
    }
    public void TakeDamage(Damage damage)
    {
        PlayerUI.Instance.ShowBloodScreen();
        Health -= damage.Value;
        PlayerHealthChanged?.Invoke();
    }
}
