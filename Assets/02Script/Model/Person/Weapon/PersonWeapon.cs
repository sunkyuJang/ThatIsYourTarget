using UnityEngine;

public class PersonWeapon : Weapon
{
    public enum WeaponType { Hands, HandGun, AR, Rifle, Non }
    public WeaponType weaponType { set; get; } = WeaponType.Non;
    public Transform LGrab;
    public Transform RGrab;
}
