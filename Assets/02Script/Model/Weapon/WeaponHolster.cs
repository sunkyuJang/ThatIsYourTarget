using UnityEngine;

public class WeaponHolster : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;
    public Weapon GetWeapon() { return weapon; }
    public void HoldWeapon(Weapon targetWeapon)
    {
        if (weapon == targetWeapon)
        {
            var targetTransform = targetWeapon.transform;
            targetTransform.SetParent(transform);
            targetTransform.localPosition = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }
    }
}
