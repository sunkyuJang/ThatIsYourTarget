using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageConnector : MonoBehaviour
{
    public DamageContorller DamageContorller { set; get; }

    private void Start()
    {
        if (DamageContorller == null)
        {
            Debug.Log("Damage controller shouldnt null");
        }
    }
    public void SetDamage(float damage, object section, out bool isDead)
    {
        DamageContorller.SetDamage(damage, this, section, out isDead);
    }
}
