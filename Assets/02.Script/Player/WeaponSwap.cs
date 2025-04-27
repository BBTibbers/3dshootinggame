using UnityEngine;

public class WeaponSwap : MonoBehaviour
{
    public GameObject Weapon1;
    public GameObject Weapon2;

    // Update is called once per frame
    void Update()
    {
        Swap();        
    }

    public void Swap()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Weapon1.SetActive(true);
            Weapon2.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        { 
            Weapon2.SetActive(true);
            Weapon1.SetActive(false);
        }
    }
}
