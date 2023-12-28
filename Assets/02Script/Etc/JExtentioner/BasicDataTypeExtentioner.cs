using UnityEngine;

namespace JExtentioner
{
    public static class BasicDataTypeExtentioner
    {
        public static float Round(this float value, int digits)
        {
            float mult = Mathf.Pow(10.0f, (float)digits);
            return Mathf.Round(value * mult) / mult;
        }
    }
}
