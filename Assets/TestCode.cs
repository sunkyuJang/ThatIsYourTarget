using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    private IEnumerator Start()
    {
        var aa = new GameObject();
        var originalAPH = GetComponent<ActionPointHandler>();
        aa.transform.position = originalAPH.transform.position;
        Debug.Log(originalAPH.transform.position);
        yield return new WaitForSeconds(3f);


        var newobj = APHManager.Instance.GetCopyAPH(originalAPH);

        newobj.transform.SetParent(aa.transform);
    }
}
