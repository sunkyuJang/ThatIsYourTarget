using UnityEditor;
using UnityEngine;

public class PersonWeapon : Weapon
{
    public enum WeaponType { Hands, HandGun, AR, Rifle, Non }
    public WeaponType weaponType { protected set; get; } = WeaponType.Non;
    public Transform grabPosition { private set; get; } = null;
    public Transform LGrab { private set; get; } = null;
    public Transform Last_L_IndexFinger { private set; get; } = null;
    public Transform Last_L_OtherFinger { private set; get; } = null;
    public Transform RGrab { private set; get; } = null;
    public Transform Last_R_IndexFinger { private set; get; } = null;
    public Transform Last_R_OtherFinger { private set; get; } = null;
    public Vector3 localPosition = Vector3.zero;
    public Vector3 localRotation = Vector3.zero;

    public void Awake()
    {
        grabPosition = transform.Find("GrabPosition");
        LGrab = grabPosition.Find("LGrab");
        //Last_L_IndexFinger = LGrab.Find("IndexFinger");
        //Last_L_OtherFinger = LGrab.Find("OtherFinger");
        RGrab = grabPosition.Find("RGrab");
        //Last_R_IndexFinger = RGrab.Find("IndexFinger");
        //Last_R_OtherFinger = RGrab.Find("OtherFinger");
    }

    public virtual void AttachToHand(Animator HumanoidAnimator) { }
}


[CustomEditor(typeof(PersonWeapon))]
public class PersonWeapon_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("for using this, u have to add 'GrabPosition' GameObj under this transform", MessageType.Info);
    }
}
