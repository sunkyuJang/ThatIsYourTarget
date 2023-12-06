using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponGrabHandler : MonoBehaviour
{
    public GameObject weaponPrefab;
    private Transform WeaponPosition { set; get; }
    private void Awake()
    {
        WeaponPosition = transform.Find("WeaponPosition");
    }
    private bool CanHold(GameObject target)
        => weaponPrefab == PrefabUtility.GetOriginalSourceRootWhereGameObjectIsAdded(target);
    public bool HoldWeapon(Weapon targetWeapon)
    {
        if (!CanHold(targetWeapon.gameObject)) return false;


        targetWeapon.transform.SetParent(WeaponPosition);
        return true;
    }
}
