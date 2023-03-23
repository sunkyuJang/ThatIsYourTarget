using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IObjDetectorConnector_OnContecting
{
    public float Dmg { set; get; } = 1;
    public float Range { set; get; } = 0;
    public bool ShouldUsingCollider { get { return Range == 0; } }
    public bool IsSingleTarget { set; get; }
    public void Attack()
    {
        if (ShouldUsingCollider)
        {
            StartCoroutine(ActiveUsingCollider());
        }
        else
        {

        }
    }
    IEnumerator ActiveUsingCollider()
    {

        yield return null;
    }
    public void OnContecting(ObjDetector detector, Collider collider)
    {
        IDamageController damageController = collider.GetComponent<IDamageController>();
        if (damageController != null)
        {
            damageController.SetDamage(Dmg, out bool isDead);
            if (isDead)
            {
                
            }
        }
    }

}
