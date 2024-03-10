using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JExtentioner;
public class ObjMover : MonoBehaviour, IObjDetectorConnector_OnDetected
{
    public GameObject MovePosition;
    public GameObject Group;
    public NavMeshAgent agent;
    public bool isStarted = false;
    public void OnDetected(ObjDetector detector, Transform target)
    {
        if (!isStarted)
        {
            isStarted = true;
            StartCoroutine(DoMove());
        }
    }

    IEnumerator DoMove()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < Group.transform.childCount; i++)
        {
            var position = Group.transform.GetChild(i).position;
            agent.SetDestination(position);

            yield return new WaitUntil(() => Vector3.Distance(transform.position.ConvertVector3To2(1), position.ConvertVector3To2(1)) < 1f);
            if (i + 1 == Group.transform.childCount) i = 0;
        }
        yield return null;
    }
}
