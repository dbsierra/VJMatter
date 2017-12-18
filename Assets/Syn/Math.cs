using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Syn
{
    public class Math
    {
        public static float Map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
        public static float Lerp(float b1, float b2, float s)
        {
            return b1 + s * (b2 - b1);
        }
        public static float GetPercentageFromRange(Vector3 v)
        {
            return GetPercentageFromRange(v.x, v.y, v.z);
        }
        public static float GetPercentageFromRange(float v, float min, float max)
        {
            if( v < min )
            {
                return 0;
            }
            if( max - min == 0 || min > max )
            {
                return 0;
            }
            return (v - min) / (max - min);
        }
    }
}

