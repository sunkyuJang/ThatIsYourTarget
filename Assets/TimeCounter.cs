using System.Collections;
using System.Collections.Generic;
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
    public TimeCountData SetTimeCounting(float maxTime, float timeUnit, GameObject requestObj)
    {
        var find = countingList.Find(x => x.requestObj.Equals(requestObj));
        if (find != null)
        {
            find.nowTime = find.maxTime;
            print("in TimeCount, the Key already exist, this key name is : " + requestObj.name);
        }
        var timeData = new TimeCountData(maxTime, timeUnit, requestObj);
        var processingTimeCounting = StartCoroutine(DoTimeCounting(timeData));
        timeData.processingTimeCounting = processingTimeCounting;

        countingList.Add(timeData);
        return timeData;
    }
    public TimeCountData GetTimeCountData(GameObject requestObj)
    {
        return countingList.Find(x => x.requestObj.Equals(requestObj));
    }
    IEnumerator DoTimeCounting(TimeCountData data)
    {
        while (data.nowTime < data.maxTime)
        {
            yield return new WaitForSeconds(data.timeUnit);
            data.nowTime += data.timeUnit;
        }
        yield return null;
    }
    public void RemoveProcessCounting(TimeCountData data)
    {
        countingList.Remove(data);
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

        public GameObject requestObj;
        public Coroutine processingTimeCounting;

        public TimeCountData(float maxTime, float timeUnit, GameObject requestObj)
        {
            this.maxTime = maxTime;
            this.timeUnit = timeUnit;
            this.requestObj = requestObj;
        }
    }
}
