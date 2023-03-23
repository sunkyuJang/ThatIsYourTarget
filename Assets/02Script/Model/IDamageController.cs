using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageController
{
    void SetDamage(float damege, out bool isDead);
}
