using System.Collections;
using UnityEngine;

public class Weapon : InteractionObj, IObjCollisionDetectorConnector_OnCollisionEnter
{
    public enum WeaponType
    {
        Fist,
        HandGun,
        Rifle,
        Stick,
        Non
    }
    [SerializeField] public float Dmg { set; get; } = 1;
    [SerializeField] public float Range { set; get; } = 0;
    [SerializeField] public bool IsMelee { get { return Range == 0; } }
    [SerializeField] public float HitPower { set; get; } = 1;

    // 1 Cycle == 1 MaxCount
    public enum CanAttackStateError { OverMaxCount, Non }
    [SerializeField] protected int curHitCount = 0;
    [SerializeField] protected int maxHitCountPerCycle = 0;
    public int LeftHitCount => maxHitCountPerCycle - curHitCount;
    [SerializeField] protected WeaponType weaponType = WeaponType.Non;
    public WeaponType GetWeaponType => weaponType;


    public bool CanAttack(out CanAttackStateError canAttackStateError)
    {
        canAttackStateError = CanAttackStateError.Non;
        if (curHitCount < maxHitCountPerCycle)
        {
            return true;
        }
        else
            canAttackStateError = CanAttackStateError.OverMaxCount;

        if (canAttackStateError != CanAttackStateError.Non)
            Debug.Log(canAttackStateError.ToString());

        return false;
    }

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
