using Unity.VisualScripting;
using UnityEngine;

namespace JExtentioner
{
    public static class Vector3Extentioner
    {
        public static Transform GetMostFarOne(this Transform center, Transform[] arry) => center.GetObjByDist(arry, true);
        public static Transform GetMostClosedOne(this Transform center, Transform[] arry) => center.GetObjByDist(arry, false);
        static Transform GetObjByDist(this Transform center, Transform[] arry, bool isMostFar)
        {
            if (arry.Length > 1)
            {
                var baseOne = arry[0];
                for (int i = 1; i < arry.Length; i++)
                {
                    var targetOne = arry[i];
                    var distFromBase = Vector3.Distance(center.position, baseOne.position);
                    var distFromTarget = Vector3.Distance(center.position, targetOne.position);

                    baseOne = isMostFar ? (distFromTarget > distFromBase ? targetOne : baseOne)
                                            : (distFromTarget < distFromBase ? targetOne : baseOne);
                }

                return baseOne;
            }
            else
            {
                Debug.Log("array size is problem");
                return null;
            }
        }
        public static bool IsArrived(this Transform center, Transform target, float allowableRange = 0f) => Vector3.Distance(center.position, target.position) <= allowableRange;
        public static float GetRotationDir(this Vector3 from, Vector3 to)
        {
            var rotateDir = Vector3.Angle(from, to);
            Vector3 crossProduct = Vector3.Cross(from, to);
            float dotProduct = Vector3.Dot(crossProduct, Vector3.up);
            var dirABS = dotProduct >= 0 ? 1 : -1;

            return rotateDir * dirABS;
        }
        public static Vector3 GetDirection(this Vector3 from, Vector3 to) => to - from;
        public static Vector3 GetOverrideX(this Vector3 original, float x) => new Vector3(x, original.y, original.z);
        public static Vector3 GetOverrideY(this Vector3 original, float y) => new Vector3(original.x, y, original.z);
        public static Vector3 GetOverrideZ(this Vector3 original, float z) => new Vector3(original.x, original.y, z);
        public static Vector3 OverrideVector(Vector3 original, Vector3 overrideVector, bool isX, bool isY, bool isZ)
        {
            return new Vector3(isX ? overrideVector.x : original.x,
                                isY ? overrideVector.y : original.y,
                                isZ ? overrideVector.z : original.z);
        }

        public static Vector2 ConvertVector3To2(this Vector3 target, int exceptiedIndex)
        {
            switch (exceptiedIndex)
            {
                case 0: return new Vector2(target.y, target.z);
                case 1: return new Vector2(target.x, target.z);
                default: return new Vector2(target.x, target.y);
            }
        }
        public static Vector3 ExceptVector3Property(this Vector3 target, int expectedIndex)
        {
            switch (expectedIndex)
            {
                case 0: // X 속성을 0으로 설정
                    return new Vector3(0, target.y, target.z);
                case 1: // Y 속성을 0으로 설정
                    return new Vector3(target.x, 0, target.z);
                case 2: // Z 속성을 0으로 설정
                    return new Vector3(target.x, target.y, 0);
                default:
                    Debug.LogError("something wrong while doing except vector3");
                    return Vector3.zero;
            }
        }

        public static Vector2 ReverseVector2(this Vector2 target)
        {
            return new Vector2(target.y, target.x);
        }
    }
}
