using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Slider healthBar;
    Enemy _myself;
    private void Start()
    {
        _myself = GetComponent<Enemy>();
        _myself.EnemyHealthChnaged += ChangeHealthBar;
        healthBar.maxValue = 1f;
        healthBar.value = (float)_myself.Health / _myself.MaxHealth;
    }

    public void ChangeHealthBar()
    {
        Debug.Log("ChangeHealthBar 호출됨");
        healthBar.value = (float)_myself.Health/_myself.MaxHealth;
    }
}
