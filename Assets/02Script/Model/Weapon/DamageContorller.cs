using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DamageContorller
{
    DamageConnector[] DamageConnectors { set; get; }
    IDamagePasser DamagePasser { set; get; }

    public DamageContorller(IDamagePasser damagePasser, Transform actor)
    {
        var rigidbodies = actor.GetComponentsInChildren<Rigidbody>();
        DamageConnectors = new DamageConnector[rigidbodies.Length];
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            var item = rigidbodies[i];
            DamageConnectors[i] = item.AddComponent<DamageConnector>();
            DamageConnectors[i].DamageContorller = this;
        }

        DamagePasser = damagePasser;
    }

    public void SetDamage(float damage, DamageConnector part, object section, out bool isDead)
    {
        DamagePasser.SetDamage(section, part, damage, out isDead);
    }
}
