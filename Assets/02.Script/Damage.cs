using UnityEngine;

public enum DamageType
{
    Bullet,
    Bomb,
    Barrel,
    Enemy,
}

public class Damage
{
    public int Value;
    public GameObject From;
    public DamageType Type;
}
