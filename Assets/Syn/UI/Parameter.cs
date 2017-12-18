using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Syn;

namespace Syn.UI
{
    public class Parameter
    {
        public float curVal;
        public float minVal;
        public float maxVal;
        public float initPer;

        public Parameter(float start, float min, float max)
        {
            curVal = start;
            minVal = min;
            maxVal = max;
            initPer = Math.GetPercentageFromRange(curVal, minVal, maxVal);
        }
    }
}
