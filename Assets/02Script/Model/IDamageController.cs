using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageConnector
{
    bool SetDamage(float damage, Action afterRagdollOn = null);
}
