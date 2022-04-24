using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    private IEnumerator Start()
    {
        var aa = new GameObject();
        yield return new WaitForSeconds(3f);
        var originalAPH = GetComponent<ActionPointHandler>();

        var newobj = APHManager.Instance.GetCopyAPH(originalAPH);

        newobj.transform.SetParent(aa.transform);
    }
}
