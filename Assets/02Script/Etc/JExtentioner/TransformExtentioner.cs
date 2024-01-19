using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace JExtentioner
{
    public static class TransformExtentioner
    {
        public static List<RaycastHit> GetAllRayHitOrderBy(this Transform center, Transform target, float dist = 0f)
        {
            var hits = GetAllRayHIts(center, target, dist).ToList();
            hits.OrderBy(x => x.distance);
            return hits;
        }
        public static RaycastHit[] GetAllRayHIts(this Transform center, Transform target, float dist = 0f)
        {
            var from = center.position;
            var to = target.position;
            var dir = from.GetDirection(to);
            dist = dist == 0f ? Vector3.Distance(from, to) : dist;

            return Physics.RaycastAll(from, dir, dist, 0, QueryTriggerInteraction.Ignore);
        }

        public static RaycastHit[] GetAllRayHIts(this Transform center, Vector3 dir, float dist = 0f)
        {
            var from = center.position;
            return Physics.RaycastAll(from, dir, dist, 0, QueryTriggerInteraction.Ignore);
        }

        public static bool IsRayHit(this Transform center, Transform target, out RaycastHit hit, float dist, float limitedAngle = 180f)
        {
            var from = center.position;
            var to = target.position;
            var dir = from.GetDirection(to);
            hit = new RaycastHit();
            for (bool isSelf = false; isSelf == false;)
            {
                if (Physics.Raycast(from, dir, out hit, dist))
                {
                    float angle = Vector3.Angle(center.forward, hit.point - center.position);
                    if (angle <= limitedAngle)
                    {
                        if (hit.transform != center && !hit.transform.IsChildOf(center.root))
                        {
                            return true;
                        }
                        else
                        {
                            var hitPoint = hit.point + dir * 0.01f;
                            dist -= Vector3.Distance(from, hitPoint);
                            from = hitPoint;
                        }
                    }
                    else
                    {
                        break;
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

        public static bool IsRayHitToTarget(this Transform center, Transform target, float limitedAngle = 180f)
        {
            var dist = Vector3.Distance(center.position, target.position);
            if (IsRayHit(center, target, out RaycastHit hit, dist, limitedAngle))
            {
                if (hit.transform.CompareTag(target.tag))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsRayHitToTarget(this Transform center, Transform target, float dist, float limitedAngle = 180f)
        {
            if (IsRayHit(center, target, out RaycastHit hit, dist, limitedAngle))
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

        public static bool IsHitToTargetWithAngleRange(this Transform center, Transform target, float maxAngle, float eachAngleUnit, float dist)
        {
            var hits = GetFrontRangeHit(center, target, maxAngle, eachAngleUnit, dist);
            foreach (var hit in hits)
            {
                if (hit.transform.CompareTag(target.tag))
                {
                    return true;
                }
            }

            return false;
        }
        public static List<RaycastHit> GetFrontRangeHit(this Transform center, Transform target, float maxAngle, float eachAngleUnit, float dist)
        {
            var hits = new List<RaycastHit>();
            var forward = target.position - center.position;
            var from = center.position;
            for (float angle = maxAngle * -1f; angle <= maxAngle; angle += eachAngleUnit)
            {
                var dir = Quaternion.Euler(0f, angle, 0f) * forward;
                GizmosDrawer.instanse.DrawLine(center.position, dir * dist + center.position, 2f, Color.grey);
                for (bool isSelf = false; isSelf == false;)
                {
                    var ray = new Ray(from, dir);
                    var targetLayer = 1 << LayerMask.NameToLayer("Ignore Raycast");
                    if (Physics.Raycast(ray, out RaycastHit hit, dist, ~targetLayer))
                    {
                        if (hit.transform != center &&
                            !hit.transform.IsChildOf(center.root))
                        {
                            hits.Add(hit);
                            break;
                        }
                        else
                        {
                            //Debug.DrawLine(from, hit.point + dir * 0.01f, Color.magenta, 2f);
                            var newFrom = hit.point + dir * 0.01f;
                            dist -= Vector3.Distance(from, newFrom);
                            from = hit.point;
                        }
                    }
                    else
                        break;
                }
            }

            return hits;
        }

        public static Transform FindStartingParentWithSameTag(this Transform target)
        {
            Transform lastParent = target;
            while (lastParent.parent != null)
            {
                var targetParent = lastParent.parent;
                if (targetParent.CompareTag(lastParent.tag))
                {
                    lastParent = targetParent;
                }
                else
                {
                    return lastParent;
                }
            }

            return null;
        }
    }
}
