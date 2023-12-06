using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HoldingWeapon<T> where T : Weapon
{
    public T WeaponClass { set; get; }
}
