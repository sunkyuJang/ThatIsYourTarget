using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IObjCollisionDetectorConnector_OnCollisionEnter
{
    public float Dmg { set; get; } = 1;
    public float Range { set; get; } = 0;
    public bool ShouldUsingCollider { get { return Range == 0; } }
    public bool IsSingleTarget { set; get; }
    public float hitPower { set; get; } = 1;
    public void Attack()
    {
        // 하는 중, 원거리와 근커리에 따른 콜라이더를 어떻게 처리할 것인가?
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
    public void OnCollisionEnter(ObjCollisionDetector detector, Collision collision)
    {
        IDamageController damageController = collision.collider.GetComponent<IDamageController>();
        if (damageController != null)
        {
            var isDead = damageController.SetDamage(Dmg);
            if (isDead)
            {
                // melee
                if (ShouldUsingCollider)
                {

                }
                // distance 
                else
                {
                    if (collision.contacts.Length > 0)
                    {
                        var hipPoint = collision.contacts[0];
                        var targetRigidbody = damageController.GetRigidbody();
                        targetRigidbody?.AddForceAtPosition((hipPoint.point - transform.position).normalized * hitPower * 100, hipPoint.point, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
