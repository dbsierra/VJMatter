using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

namespace Syn.UI
{
    public class UI
    {
        /*
        private struct Param
        {
            public float curVal;
            public float minVal;
            public float maxVal;
            public float initPer;

            public Param(float c, float min, float max, float i)
            {
                curVal = c;
                minVal = min;
                maxVal = max;
                initPer = i;
            }
        }
        */
        Dictionary<int, Parameter> parameters;

        public UI()
        {
            parameters = new Dictionary<int, Parameter>();
        }

        public void RegisterParameter(int MIDI, float initVal, float min, float max)
        {
            parameters[MIDI] = new Parameter(initVal, min, max);
        }

        public void RegisterParameter(int MIDI, Vector3 param)
        {
            parameters[MIDI] = new Parameter(param.x, param.y, param.z);
        }

        public float GetParameterValue(int MIDI, Vector3 param)
        {
            UpdateParameter(MIDI, param);
            return GetParameterValue(MIDI);
        }

        public float GetParameterValue(int MIDI)
        {
            Parameter p = parameters[MIDI];
            parameters[MIDI].curVal = Mathf.Lerp(p.minVal, p.maxVal, MidiMaster.GetKnob(MIDI, p.initPer));
            return parameters[MIDI].curVal;
        }

        public void UpdateParameter(int MIDI, Vector3 param)
        {
            parameters[MIDI].minVal = param.y;
            parameters[MIDI].maxVal = param.z;
        }


    }
}
