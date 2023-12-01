using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
public class PhysicsTrafficHandler
{
    private Dictionary<Vector3, PhysicsTrafficData> trafficData = new Dictionary<Vector3, PhysicsTrafficData>();
    private float cacheDuration = 2f; // 캐시된 결과 유지 시간

    public bool IsCongested(Vector3 location, float radius, NaviController requester)
    {
        CleanupExpiredCache();

        if (IsLocationCached(location, out Vector3 keyPosition))
        {
            var data = trafficData[keyPosition];
            //if (data.firstRequester.Equals(requester)) return data.IsCongested;

            var tooManyAccessed = data.AccessedCount++ > 5;
            if (!tooManyAccessed)
                data.LastCheckTime -= 0.5f;
            data.IsCongested = tooManyAccessed;
            return data.IsCongested;
        }

        Collider[] hitColliders = Physics.OverlapSphere(location, radius, LayerMask.GetMask("Actor"));
        for (int i = 0; i < hitColliders.Length; i++)
        {
            var agent = hitColliders[i].GetComponent<NavMeshAgent>();
            if (agent != null) agent.avoidancePriority = (agent.avoidancePriority + i) % 99;
        }
        bool isCongested = hitColliders.Length > 3;
        trafficData[location] = new PhysicsTrafficData(requester, isCongested, Time.time);
        GizmosDrawer.instanse.DrawSphere(location, radius, 2f, Color.red - new Color(0, 0, 0, 0.7f));
        return isCongested;
    }

    private bool IsLocationCached(Vector3 location, out Vector3 keyPosition)
    {
        foreach (var entry in trafficData)
        {
            if ((location - entry.Key).magnitude <= 2f && Time.time - entry.Value.LastCheckTime <= cacheDuration)
            {
                keyPosition = entry.Key;
                return true;
            }
        }

        keyPosition = Vector3.zero;
        return false;
    }

    private void CleanupExpiredCache()
    {
        List<Vector3> keysToRemove = new List<Vector3>();

        foreach (var entry in trafficData)
        {
            if (Time.time - entry.Value.LastCheckTime > cacheDuration)
            {
                keysToRemove.Add(entry.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            trafficData.Remove(key);
        }
    }

    private class PhysicsTrafficData
    {
        public NaviController firstRequester;
        public bool IsCongested;
        public float LastCheckTime;
        public int AccessedCount = 0;

        public PhysicsTrafficData(NaviController requester, bool isCongested, float lastCheckTime)
        {
            firstRequester = firstRequester == null ? requester : firstRequester;
            IsCongested = isCongested;
            LastCheckTime = lastCheckTime;
        }
    }
}