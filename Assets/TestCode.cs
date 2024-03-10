using System.Collections;
using UnityEngine;
using JExtentioner;
using System.Linq;

public class TestCode : MonoBehaviour, IObjDetectorConnector_OnDetected
{
    public void OnDetected(ObjDetector detector, Transform target)
    {
        var radius = 10f;
        var force = 1000f;
        var damage = 10f;
        var list = Physics.OverlapSphere(transform.position, radius).ToList();

        list.ForEach(x =>
        {
            var rigi = x.GetComponent<Rigidbody>();
            if (rigi != null)
            {
                var damageConnector = rigi.GetComponent<DamageConnector>();
                if (damageConnector != null)
                {
                    damageConnector.SetDamage(10f, this, out bool isDead);
                }

                rigi.AddExplosionForce(force, transform.position, radius);
            }
        });
    }
}
