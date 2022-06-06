using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMath
{
    public static class Vector3Extentioner
    {
        public static Transform GetMostFarOne(Transform center, Transform[] arry) => GetObjByDist(center, arry, true);
        public static Transform GetMostClosedOne(Transform center, Transform[] arry) => GetObjByDist(center, arry, false);
        static Transform GetObjByDist(Transform center, Transform[] arry, bool isMostFar)
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
        public static bool IsArrived(Transform center, Transform target, float marginOfError = 0f) => Vector3.Distance(center.position, target.position) <= marginOfError;


        public static Vector3 GetOverrideX(Vector3 original, float x) => OverrideVector(original, new Vector3(x, 0, 0), true, false, false);
        public static Vector3 GetOverrideY(Vector3 original, float y) => OverrideVector(original, new Vector3(0, y, 0), false, true, false);
        public static Vector3 GetOverrideZ(Vector3 original, float z) => OverrideVector(original, new Vector3(0, 0, z), false, false, true);
        public static Vector3 OverrideVector(Vector3 original, Vector3 overrideVector, bool isX, bool isY, bool isZ)
        {
            return new Vector3(isX ? overrideVector.x : original.x,
                                isY ? overrideVector.y : original.y,
                                isZ ? overrideVector.z : original.z);
        }
    }
}
