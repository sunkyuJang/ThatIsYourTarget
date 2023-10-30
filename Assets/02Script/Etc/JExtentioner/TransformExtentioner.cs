using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace JExtentioner
{
    public static class TransformExtentioner
    {
        public static RaycastHit[] GetAllRayHIts(this Transform center, Transform target, float dist = 0f)
        {
            var from = center.position;
            var to = target.position;
            var dir = from.GetDirection(to);
            dist = dist == 0f ? Vector3.Distance(from, to) : dist;

            return Physics.RaycastAll(from, dir, dist, 0, QueryTriggerInteraction.Ignore);
        }

        public static bool IsRayHit(this Transform center, Transform target, out RaycastHit hit, float dist = 0f)
        {
            var from = center.position;
            var to = target.position;
            var dir = from.GetDirection(to);
            dist = dist == 0f ? Vector3.Distance(from, to) : dist;
            hit = new RaycastHit();
            for (bool isSelf = false; isSelf == false;)
            {
                if (Physics.Raycast(from, dir, out hit, dist))
                {
                    if (hit.transform != center &&
                        !hit.transform.IsChildOf(center.root))
                    {
                        return true;
                    }
                    else
                    {
                        //Debug.DrawLine(from, hit.point + dir * 0.01f, Color.magenta, 2f);
                        from = hit.point + dir * 0.01f;
                        dist = Vector3.Distance(from, to);
                    }
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        public static bool IsRayHit(this Transform center, Vector3 dir, out RaycastHit hit, float dist)
        {
            var from = center.position;
            hit = new RaycastHit();
            for (bool isSelf = false; isSelf == false;)
            {
                if (Physics.Raycast(from, dir, out hit, dist))
                {
                    if (hit.transform != center &&
                        !hit.transform.IsChildOf(center.root))
                    {
                        return true;
                    }
                    else
                    {
                        //Debug.DrawLine(from, hit.point + dir * 0.01f, Color.magenta, 2f);
                        var newFrom = hit.point + dir * 0.01f;
                        dist -= Vector3.Distance(from, newFrom);
                    }
                }
                else
                {
                    break;
                }
            }
            return false;
        }

        public static bool IsRayHitToTarget(this Transform center, Transform target, float dist = 0f)
        {
            if (IsRayHit(center, target, out RaycastHit hit, dist))
            {
                if (hit.transform.CompareTag(target.tag))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Vector3> GetSurroundingCastHitPosition(this Transform center, float eachAngleUnit, float dist, bool shouldAddOnlyHit = true)
        {
            var hits = new List<Vector3>();
            var forward = center.forward;
            for (float maxAngle = 180f, angle = maxAngle * -1f; angle < maxAngle; angle += eachAngleUnit)
            {
                var dir = Quaternion.Euler(0f, angle, 0f) * forward;
                var ray = new Ray(center.position, dir);
                if (Physics.Raycast(ray, out RaycastHit hit, dist))
                {
                    hits.Add(hit.point);
                }
                else
                {
                    if (!shouldAddOnlyHit)
                    {
                        hits.Add(ray.GetPoint(dist));
                    }
                }
            }

            return hits;
        }
    }
}
