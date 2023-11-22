using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JExtentioner;
using UnityEngine.AI;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;

public class NaviTrafficManager : MonoBehaviour
{
    public static NaviTrafficManager Instance { set; get; }
    List<TrafficData> TrafficDatas { set; get; } = new List<TrafficData>();
    float CastRadius = 2f;
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

    public bool AddCasePoint(Vector3 targetPosition, NaviController naviController, AnimationPoint ap)
    {
        var targetData = TrafficDatas.Find(x =>
        {
            var dist = Vector3.Distance(x.Position, targetPosition);
            return dist < CastRadius;
        });

        if (targetData == null)
        {
            var data = new TrafficData();
            data.AddingControllers(naviController, ap);
            TrafficDatas.Add(data);
            StartCoroutine(HandlingTraffic(data));

            return true;
        }

        return targetData.AddingControllers(naviController, ap);
    }

    IEnumerator HandlingTraffic(TrafficData trafficData)
    {
        while (trafficData.NaviPairAP.Count > 0)
        {
            var pair = trafficData.NaviPairAP.Dequeue();
            var naviController = pair.Key;
            var agent = naviController.navMeshAgent;
            var ap = pair.Value;

            if (Vector3.Distance(ap.transform.position, agent.nextPosition) > CastRadius * 2f)
                continue;

            agent.isStopped = false;
            agent.avoidancePriority = 0;
            yield return new WaitUntil(() => Vector3.Distance(trafficData.Position, agent.transform.position) > CastRadius * 1f);
        }

        TrafficDatas.Remove(trafficData);
        yield return null;
    }

    public class TrafficData
    {
        public Vector3 Position { set; get; }
        public Queue<KeyValuePair<NaviController, AnimationPoint>> NaviPairAP { set; get; } = new Queue<KeyValuePair<NaviController, AnimationPoint>>();
        public bool isUnlimitedPlayingExist = false;
        public bool AddingControllers(NaviController naviController, AnimationPoint ap)
        {
            if (NaviPairAP.Count > 3)
                return false;
            if (isUnlimitedPlayingExist)
                return false;

            if (NaviPairAP.Any(pair => pair.Key == naviController))
            {
                return false;
            }
            else
            {
                Position = ap.transform.position;
                NaviPairAP.Enqueue(new KeyValuePair<NaviController, AnimationPoint>(naviController, ap));
                var agent = naviController.navMeshAgent;
                agent.avoidancePriority = NaviPairAP.Count;
                agent.isStopped = true;

                if (ap.IsUnLimited)
                {
                    isUnlimitedPlayingExist = true;
                }

                return true;
            }
        }
    }
}
