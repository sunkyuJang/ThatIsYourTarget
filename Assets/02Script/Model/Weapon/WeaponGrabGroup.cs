using System.Collections.Generic;
using UnityEngine;

public class WeaponGrabSoketHandler : MonoBehaviour
{
    List<Transform> weaponsSoket { set; get; } = new List<Transform>();

    private void Awake()
    {
        for (int i = 0; i < weaponsSoket.Count; i++)
        {

        }
    }
}
