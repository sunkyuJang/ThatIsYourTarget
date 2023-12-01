using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MetaphysicsTrafficHandler
{
    private List<TrafficData> trafficDatas = new List<TrafficData>();

    public bool IsCongested(Vector3 targetPosition, NaviController naviController, float castRadius, out TrafficData data)
    {
        data = null;
        var targetData = trafficDatas.Find(x =>
        {
            var dist = Vector3.Distance(x.Position, targetPosition);
            return dist < castRadius;
        });

        if (targetData == null)
        {
            data = new TrafficData();
            return false;
        }

        var canAdd = targetData.CanAdd(naviController);
        data = targetData;

        return !canAdd;
    }

    public void AddData(TrafficData data)
    {
        if (data.IsNew)
        {
            data.IsNew = false;
            trafficDatas.Add(data);
            NaviTrafficManager.Instance.StartCoroutine(HandlingTraffic(data));
        }
    }

    IEnumerator HandlingTraffic(TrafficData trafficData)
    {
        while (trafficData.NaviPairAP.Count > 0)
        {
            var pair = trafficData.NaviPairAP[0];
            var naviController = pair.Key;
            var agent = naviController.navMeshAgent;
            var registedAP = pair.Value;
            var isAnimationDone = false;

            agent.stoppingDistance = 0;
            registedAP.whenAnimationEnd += () => { isAnimationDone = true; };

            yield return new WaitUntil(() => naviController.playingAP != registedAP || isAnimationDone || trafficData.RemoveList.Contains(naviController));

            trafficData.NaviPairAP.RemoveAt(0);
            if (trafficData.RemoveList.Contains(naviController))
            {
                trafficData.RemoveList.Remove(naviController);
            }
            agent.avoidancePriority = 0;

            yield return new WaitUntil(() => !trafficData.nowAdding);
        }

        trafficDatas.Remove(trafficData);
        trafficData.isDone = true;
        yield return null;
    }
    [Serializable]
    // 기타 필요한 메서드 및 로직
    public class TrafficData
    {
        public Vector3 Position { set; get; }
        public List<KeyValuePair<NaviController, AnimationPoint>> NaviPairAP { set; get; } = new List<KeyValuePair<NaviController, AnimationPoint>>();
        public List<NaviController> RemoveList = new List<NaviController>();
        public bool isUnlimitedPlayingExist = false;
        public int AddingCount { set; get; } = 1;
        public bool IsNew = true;
        public bool isDone = false;
        public bool nowAdding = false;
        public bool AddingControllers(NaviController naviController, AnimationPoint ap, Vector3 position)
        {
            nowAdding = true;
            Position = position;
            NaviPairAP.Add(new KeyValuePair<NaviController, AnimationPoint>(naviController, ap));
            var agent = naviController.navMeshAgent;
            agent.avoidancePriority = AddingCount++;
            agent.stoppingDistance = 2f;

            if (ap.IsUnLimited)
            {
                isUnlimitedPlayingExist = true;
            }

            nowAdding = false;
            return true;
        }

        public void RemoveControllers(NaviController naviController)
        {
            var agent = naviController.navMeshAgent;
            RemoveList.Add(naviController);
            agent.stoppingDistance = 0f;
            agent.avoidancePriority = 0;
        }

        public bool CanAdd(NaviController naviController)
        {
            // for (int i = 0; i < NaviPairAP.Count; i++)
            // {
            //     var pair = NaviPairAP[i];
            //     var navi = pair.Key;
            //     if (navi.Equals(naviController)) return i != 0;
            // }
            if (AddingCount > 4) return false;
            if (isUnlimitedPlayingExist) return false;
            if (NaviPairAP.Any(pair => pair.Key == naviController)) return false;

            return true;
        }

        public void OrganizeList()
        {
            NaviPairAP.Sort((pair1, pair2) =>
            {
                float distance1 = Vector3.Distance(pair1.Key.transform.position, pair1.Value.CorrectedPosition);
                float distance2 = Vector3.Distance(pair2.Key.transform.position, pair2.Value.CorrectedPosition);
                return distance1.CompareTo(distance2);
            });
        }
    }
}