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

        public static RaycastHit GetRayHit(this Transform center, Transform target, float dist = 0f)
        {
            var from = center.position;
            var to = target.position;
            var dir = from.GetDirection(to);
            dist = dist == 0f ? Vector3.Distance(from, to) : dist;

            Physics.Raycast(from, dir, out RaycastHit hit, dist);
            return hit;
        }

        public static bool IsRayHitToTarget(this Transform center, Transform target, float dist = 0f)
        {
            var hit = GetRayHit(center, target, dist);
            return hit.transform == null ? false : true;
        }
    }
}
