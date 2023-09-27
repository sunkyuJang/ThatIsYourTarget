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

        public static bool IsRayHitToTarget(this Transform center, Transform target, float dist = 0f)
        {
            if (IsRayHit(center, target, out RaycastHit hit, dist))
            {
                return hit.transform.CompareTag(target.tag);
            }

            return false;
        }
    }
}
