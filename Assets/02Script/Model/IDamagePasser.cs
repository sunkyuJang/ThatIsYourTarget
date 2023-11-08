using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagePasser
{
    public void SetDamage(object section, object parts, float damage, out bool isDead);
}
