using System.Collections.Generic;
using UnityEngine;

public class WeaponHolsterManager : MonoBehaviour
{
    List<WeaponHolster> weaponHolsters = new List<WeaponHolster>();
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var holster = transform.Find(typeof(WeaponHolster).Name).GetComponent<WeaponHolster>();
            if (holster != null)
            {
                weaponHolsters.Add(holster);
            }
        }
    }
}
