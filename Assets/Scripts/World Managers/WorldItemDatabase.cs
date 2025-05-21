using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldItemDatabase : MonoBehaviour
{
    public WeaponItem unarmedWeapon;
    [Header("Weapons")]
    public static WorldItemDatabase instance;
    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();
    // A LIST OF EVERY ITEM WE HAVE IN THE GAME
    [SerializeField] List<Item> items = new List<Item>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }
        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }




    }

    public WeaponItem GetWeaponByID(int id)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == id);
    }

}
