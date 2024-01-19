using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    public static TimeCounter Instance { set; get; }
    List<TimeCountData> countingList = new List<TimeCountData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            print("there has two TimeCounter Exist");
        }
    }
    public TimeCountData SetTimeCounting(float maxTime, Action function, object sequenceKey = null, Func<object, bool> sequnceMatch = null)
    {
        return SetTimeCounting(maxTime, maxTime, function, sequenceKey, sequnceMatch);
    }
    TimeCountData SetTimeCounting(float maxTime, float timeUnit, Action function, object sequenceKey, Func<object, bool> sequnceMatch)
    {
        var find = countingList.Find(x => x.requestFunction.Equals(function));
        if (find != null)
        {
            find.nowTime = find.maxTime;
            print("in TimeCount, the Key already exist, this key name is : " + function);
        }
        var timeData = new TimeCountData(maxTime, timeUnit, function, sequenceKey, sequnceMatch);
        var processingTimeCounting = StartCoroutine(DoTimeCounting(timeData));
        timeData.processingTimeCounting = processingTimeCounting;

        countingList.Add(timeData);
        return timeData;
    }
    IEnumerator DoTimeCounting(TimeCountData data)
    {
        while (data.nowTime < data.maxTime)
        {
            yield return new WaitForSeconds(data.timeUnit);
            data.nowTime += data.timeUnit;
        }

        if (data.sequence != null)
        {
            if (data.sequnceMatch(data.sequence))
            {
                data.requestFunction.Invoke();
            }
        }
        else
        {
            data.requestFunction.Invoke();
        }

        countingList.Remove(data);
        yield return null;
    }

    public class TimeCountData
    {
        public float maxTime;
        public float nowTime;
        public float timeUnit;
        public bool IsCounting
        {
            get { return nowTime < maxTime; }
        }

        public Action requestFunction;
        public Coroutine processingTimeCounting;
        public object sequence = null;
        public Func<object, bool> sequnceMatch = null;

        public TimeCountData(float maxTime, float timeUnit, Action requestFunc, object sequence = null, Func<object, bool> sequenceMatfch = null)
        {
            this.maxTime = maxTime;
            this.timeUnit = timeUnit;
            this.requestFunction = requestFunc;
            this.sequence = sequence;
            this.sequnceMatch = sequenceMatfch;
        }
    }
}
