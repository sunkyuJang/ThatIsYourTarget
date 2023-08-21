using System.Collections;
using UnityEngine;


namespace JExtentioner
{
    public static class WaitUntilExtentioner
    {
        public static IEnumerator WaitUntilWithFixedTime(System.Func<bool> predicate)
        {
            while (!predicate())
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
