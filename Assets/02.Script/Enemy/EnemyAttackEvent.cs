using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyAttackEvent : MonoBehaviour
{
    public Enemy MyEnemy;

    public void AttackEvent()
    {
        MyEnemy.Hit();
        Debug.Log("공격!");
    }
}
