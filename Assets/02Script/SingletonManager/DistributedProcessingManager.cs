using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// this func will work for DistributedProcessing
/// after adding job, will functioning after 1 frame later.
/// </summary>
public class DistributedProcessingManager : MonoBehaviour
{
    public static DistributedProcessingManager Instance;
    [SerializeField] private int jobCountForEachFrame = 10;
    private Dictionary<object, Queue<Action>> jobList = new Dictionary<object, Queue<Action>>();
    private Coroutine Proc_InitiateJob { set; get; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddJob(object section, Queue<Action> actions)
    {
        if (actions == null) return;

        if (!jobList.ContainsKey(section))
        {
            jobList.Add(section, new Queue<Action>());
        }

        foreach (var action in actions)
        {
            jobList[section].Enqueue(action);
        }

        if (Proc_InitiateJob == null)
            Proc_InitiateJob = StartCoroutine(InitiateJob());
    }

    IEnumerator InitiateJob()
    {
        // giving Grace time for AddJob Func.
        yield return null;

        var removeSection = new List<object>();
        while (0 < jobList.Count)
        {
            var keys = jobList.Keys;
            for (int i = 0; i < keys.Count; ++i)
            {
                var key = keys.ElementAt(i);
                var eachQueue = jobList[key];
                var maxCount = jobCountForEachFrame < eachQueue.Count ? jobCountForEachFrame : eachQueue.Count;
                for (int j = 0; j < maxCount; j++)
                {
                    eachQueue.Dequeue()?.Invoke();
                }

                if (eachQueue.Count <= 0)
                {
                    if (!removeSection.Contains(key))
                        removeSection.Add(key);
                }
            }
            // giving Grace time for AddJob Func.
            yield return null;
            for (int k = 0; k < removeSection.Count; ++k)
            {
                var key = removeSection[k];
                if (jobList[key].Count <= 0)
                    jobList.Remove(key);
            }
            removeSection.Clear();
        }

        Proc_InitiateJob = null;
        yield break;
    }
}
