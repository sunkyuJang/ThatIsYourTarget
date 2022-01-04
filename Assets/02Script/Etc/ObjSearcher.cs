using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSearcher : MonoBehaviour
{
    public List<string> targetTags = new List<string>();
    public bool shouldFindByName = false;

    public float castingRadius = 0.1f;
    public Color castColor = Color.red;
    List<ObjSearcherRecoardUnit> drawTimes = new List<ObjSearcherRecoardUnit>();
    public List<Collider> GetMultipleTargetWithMarking(float markingTime)
    {
        drawTimes.Add(new ObjSearcherRecoardUnit(transform.position, castingRadius, markingTime));
        return GetMultipleTarget();
    }
    public List<Collider> GetMultipleTarget()
    {
        var hits = Physics.OverlapSphere(transform.position, castingRadius);
        List<Collider> objList = null;
        if (hits.Length > 0)
        {
            objList = new List<Collider>();
            foreach (var item in hits)
            {
                var nowString = shouldFindByName ? item.gameObject.name : item.tag;
                foreach (var targetString in targetTags)
                {
                    if (nowString.Equals(targetString))
                    {
                        objList.Add(item);
                    }
                }
            }
        }

        return objList;
    }

    public Collider GetSingleTarget()
    {
        var ObjList = GetMultipleTarget();
        Collider target = null;

        if (ObjList != null)
        {
            if (ObjList.Count == 1)
            {
                return ObjList[0];
            }
            else
            {
                var mostCloseObj = ObjList[0];
                var mostCloseDist = Vector3.Distance(transform.position, mostCloseObj.transform.position);

                for (int i = 1; i < ObjList.Count; i++)
                {
                    var nowObj = ObjList[i];
                    var nowDist = Vector3.Distance(transform.position, nowObj.transform.position);
                    if (nowDist < mostCloseDist)
                    {
                        mostCloseObj = nowObj;
                        mostCloseDist = nowDist;
                    }
                }

                target = mostCloseObj;
            }
        }

        return target;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = castColor;
        if (drawTimes.Count == 0)
            Gizmos.DrawWireSphere(transform.position, castingRadius);
        else
        {
            for (int i = 0; i < drawTimes.Count; i++)
            {
                var nowUnit = drawTimes[i];
                if (nowUnit.time == 0)
                {
                    drawTimes.RemoveAt(i--);
                    continue;
                }
                else
                {
                    Gizmos.DrawSphere(nowUnit.position, nowUnit.radius);
                    nowUnit.time -= Time.deltaTime;
                }
            }
        }
    }

    class ObjSearcherRecoardUnit
    {
        public Vector3 position;
        public float radius;
        public float time;
        public ObjSearcherRecoardUnit(Vector3 position, float radius, float time)
        {
            this.position = position;
            this.radius = radius;
            this.time = time;
        }
    }
}
