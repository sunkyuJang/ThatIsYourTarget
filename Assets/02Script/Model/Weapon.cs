using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour, IObjCollisionDetectorConnector_OnCollisionEnter
{
    public float Dmg { set; get; } = 1;
    public float Range { set; get; } = 0;
    public bool IsMelee { get { return Range == 0; } }
    public bool IsSingleTarget { set; get; }
    public float HitPower { set; get; } = 1;
    public int HitPerCycle { set; get; } = 0;
    public float CoolPerHit { set; get; } = 0;
    public float CoolPerCycle { set; get; } = 0;
    public int CurHitCount { set; get; } = 0;

    public void Attack()
    {
        // 하는 중, 원거리와 근거리에 따른 콜라이더를 어떻게 처리할 것인가?
        if (IsMelee)
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

    public void OnCollisionEnterByConnector(ObjCollisionDetector detector, Collision collision)
    {
        IDamageConnector damageController = collision.collider.GetComponent<IDamageConnector>();
        if (damageController != null)
        {
            var isDead = damageController.SetDamage(Dmg, () => { });
            if (isDead)
            {
                // melee
                if (IsMelee)
                {

                }
                // distance 
                else
                {
                    if (collision.contacts.Length > 0)
                    {
                        var hipPoint = collision.contacts[0];
                        var targetRigidbody = collision.rigidbody;
                        targetRigidbody?.AddForceAtPosition((hipPoint.point - transform.position).normalized * HitPower * 100, hipPoint.point, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
