using System.Collections;
using UnityEngine;

public class TestCode : MonoBehaviour, IObjDetectorConnector_OnDetected
{
    public void OnDetected(ObjDetector detector, Collider collider)
    {
        if (collider.tag == "Person")
        {
            StartCoroutine(DoMove());
        }
    }
    IEnumerator DoMove()
    {
        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(3, 0.8f, -2);
    }
}
