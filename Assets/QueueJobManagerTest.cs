using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueJobManagerTest : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(Test_AddJob());
    }

    // AddJob 메서드를 호출하여 테스트할 함수입니다.
    public IEnumerator Test_AddJob()
    {
        // 테스트 데이터 준비
        var section1 = new object();
        var section2 = new object();
        var section3 = new object();
        Queue<Action> actions1 = new Queue<Action>();
        Queue<Action> actions2 = new Queue<Action>();
        Queue<Action> actions3 = new Queue<Action>();

        for (int i = 0; i < 20; i++)
        {
            var val = i;
            actions1.Enqueue(() => Debug.Log("Job1: " + val));
            if (i < 5)
                actions2.Enqueue(() => Debug.Log("Job2: " + val));
            actions3.Enqueue(() => Debug.Log("Job3: " + val));
        }

        // AddJob 메서드를 호출하여 데이터를 추가합니다.
        DistributedProcessingManager.Instance.AddJob(section1, actions1);
        DistributedProcessingManager.Instance.AddJob(section2, actions2);
        DistributedProcessingManager.Instance.AddJob(section3, actions3);

        // Coroutine을 사용하여 InitiateJob 코루틴이 실행된 후 테스트합니다.
        yield return null;
        yield return null;
        yield return null;

        // 2번째 section에 10개의 큐를 추가합니다.
        actions2.Clear();
        for (int i = 0; i < 10; i++)
        {
            var val = i;
            actions2.Enqueue(() => Debug.Log("New Job2: " + val));
        }
        DistributedProcessingManager.Instance.AddJob(section2, actions2);

        // Coroutine을 사용하여 InitiateJob 코루틴이 실행된 후 테스트합니다.
        yield return new WaitForEndOfFrame();
    }
}

