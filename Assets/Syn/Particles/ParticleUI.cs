using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;
using Syn;
using Syn.UI;

namespace Syn.Particles
{
    [RequireComponent(typeof(Animator))]
    public class ParticleUI : MonoBehaviour
    {
        public ParticleSystem PS;

        public bool MIDIEnabled = false;
        bool ready = false;

        [Header("x=current value  -  y=min value  -  z=max value")]
        public Vector3 VelMult;
        public Vector3 ShapePercentage;
        public Vector3 SizeMax;
        public Vector3 SizePow;
        public Vector3 Brightness;
        public Vector3 PulserMag;
        public Vector3 AntiPulserMag;
        public Vector3 LissajouMult;
        public Vector3 LissajouRadius;
        public Vector3 PulserTargetX;
        public float BrightnessOverdrive = 1.0f;

        [Header("Modes")]
        public int Shape;

        [Header("Textures")]
        [Range(0.0f,1.0f)]
        public float TextureLerp;
        public Texture2D[] Textures;

        private UI.UI ui;
        private Animator AC;

        // Use this for initialization
        void Start()
        {
            //Register parameters
            ui = new UI.UI();
            ui.RegisterParameter(1, VelMult);
            ui.RegisterParameter(2, Brightness);
            ui.RegisterParameter(3, PulserMag);
            ui.RegisterParameter(4, AntiPulserMag);

          //  ui.RegisterParameter(25, PulserTargetX);

         //   ui.RegisterParameter(26, LissajouMult);
          //  ui.RegisterParameter(27, LissajouRadius);

          //  ui.RegisterParameter(41, ShapePercentage);
          //  ui.RegisterParameter(42, SizeMax);
          //  ui.RegisterParameter(43, SizePow);

            AC = GetComponent<Animator>();

            ready = true;
        }

        // Update is called once per frame
        void Update()
        {
            if( ready )
            {
                if (MIDIEnabled)
                {
                    VelMult.x = ui.GetParameterValue(1, VelMult);
                    Brightness.x = ui.GetParameterValue(2, Brightness) * BrightnessOverdrive;
                    PulserMag.x = ui.GetParameterValue(3, PulserMag);
                    AntiPulserMag.x = ui.GetParameterValue(4, AntiPulserMag);

                 //   PulserTargetX.x = ui.GetParameterValue(25, PulserTargetX);
                 //   LissajouMult.x = ui.GetParameterValue(26, LissajouMult);
                  //  LissajouRadius.x = ui.GetParameterValue(27, LissajouRadius);

                    //    ShapePercentage.x = ui.GetParameterValue(41, ShapePercentage);
                    //   SizeMax.x = ui.GetParameterValue(42, SizeMax);
                    //   SizePow.x = ui.GetParameterValue(43, SizePow);

                }

                PS.SetVelocityMult(VelMult.x);
                PS.SetShape(Shape);
                PS.SetShapePercentage(ShapePercentage.x);
                PS.SetSizeMax(SizeMax.x);
               // PS.SetSizePow(SizePow.x);
                PS.SetBrightness(Brightness.x);
                PS.SetPulserMag(PulserMag.x);
                PS.SetAntiPulserMag(AntiPulserMag.x);
                PS.SetLissForceMult(LissajouMult.x);
                PS.SetLissRadius(LissajouRadius.x);
                PS.SetPulserTargetX(PulserTargetX.x);

                PS.SetGradientLerp(TextureLerp);
            }
        }

        public void NewTargetColor(Texture2D tex)
        {
            if( TextureLerp > 0 )
            {
                PS.SetTexture1(tex);
            }
            else
            {
                PS.SetTexture2(tex);
            }

            AC.SetTrigger("Swap");
        }

    }
}

