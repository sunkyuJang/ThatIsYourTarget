using UnityEditor;
using UnityEngine;

public class WeaponHolster : MonoBehaviour
{
    public GameObject weaponPrefab;
    private Transform HandGroup { set; get; }
    private Transform L_Hand { set; get; }
    private Transform R_Hand { set; get; }
    private bool CanHold(GameObject target)
        => weaponPrefab == PrefabUtility.GetOriginalSourceRootWhereGameObjectIsAdded(target);

    [SerializeField]
    private Weapon weapon;
    public Weapon GetWeapon() { return weapon; }
    public bool HoldWeapon(Weapon targetWeapon)
    {
        if (!CanHold(weapon.gameObject)) return false;

        targetWeapon.transform.SetParent(R_Hand);
        return true;
        if (weapon == targetWeapon)
        {
            // var targetTransform = targetWeapon.transform;
            // targetTransform.SetParent(transform);
            // targetTransform.localPosition = Vector3.one;
            // transform.localRotation = Quaternion.identity;


        }
    }
}
