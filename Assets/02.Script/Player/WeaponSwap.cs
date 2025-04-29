using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class WeaponSwap : MonoBehaviour
{
    public GameObject Weapon1;
    public GameObject Weapon2;
    public GameObject Weapon3;

    public GameObject CrossHair1;
    public GameObject CrossHair2;
    public GameObject CrossHair3;

    public Sprite Sprite1;
    public Sprite Sprite2;
    public Sprite Sprite3;

    public Image Icon;

    public List<GameObject> Weapons = new List<GameObject>();
    public List<GameObject> CrossHairs = new List<GameObject>();
    public List<Sprite> Sprites = new List<Sprite>();
    private int _currentWeaponIndex = 0;

    // Update is called once per frame
    void Start()
    {
        Weapons.Add(Weapon1);
        Weapons.Add(Weapon2);
        Weapons.Add(Weapon3);

        CrossHairs.Add(CrossHair1);
        CrossHairs.Add(CrossHair2);
        CrossHairs.Add(CrossHair3);

        Sprites.Add(Sprite1);
        Sprites.Add(Sprite2);
        Sprites.Add(Sprite3);
    }
    void Update()
    {
        Swap();        
    }

    public void Swap()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentWeaponIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentWeaponIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _currentWeaponIndex = 2;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            _currentWeaponIndex = (_currentWeaponIndex + 1) % Weapons.Count;
        }
        else if (scroll < 0f)
        {
            _currentWeaponIndex = (_currentWeaponIndex - 1 + Weapons.Count) % Weapons.Count;
        }

        for (int i = 0; i < Weapons.Count; i++)
        {
            Weapons[i].SetActive(i == _currentWeaponIndex);
            CrossHairs[i].SetActive(i == _currentWeaponIndex);
            Icon.sprite = Sprites[_currentWeaponIndex];
        }
    }
}
