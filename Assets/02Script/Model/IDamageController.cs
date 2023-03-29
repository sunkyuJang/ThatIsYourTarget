using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageController
{
    bool SetDamage(float damege);
    void SetDestroying();
    void PushByForce(Vector3 force, Vector3 hitPosition, ForceMode forceMode);
}
