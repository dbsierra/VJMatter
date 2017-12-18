using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Syn;
using Syn.UI;
using Skinner;

public class SkinnerUI : MonoBehaviour {

    UI ui;



    // Parameters

    // Particles

    //ShardUp
    // Shrd.Probability (.5 - 1)
    // Shrd.Particle scale (.05 - .2)
    // 
    //
    //Mega
    // Shrd.Gravity -.4 to 6
    // Shrd.Noise Amplitude .4 to 4
    // Speed Limit (.4-11)
    // Sensitivity (.02 - .4)
    // Brightness (2 - 5)
    //
    //SlowMo
    // Shrd.Drag (2-15)
    // Shrd.MaxLife (2-4)
    //
    //Hueness
    // Shrd.BaseHue (0 - 1)
    // Tril.BaseHue (0 - 1)
    //
    //Glitch
    // 
    // 
    // Trail
    //Brightness

    //Ghost
    // TrailMat.Saturation

    // Glitch
    //Area threshold

    public Vector3 Mega;
    public Vector3 ShardUp;
    public Vector3 SlowMo;
    public Vector3 Hueness;

    public SkinnerTrail Trail;
    public SkinnerParticle Shards;
    public Material TrailMat;
    public Material ShardsMat;

	// Use this for initialization
	void Start () {
        //Register parameters
        ui = new UI();
        ui.RegisterParameter(21, Mega);
        ui.RegisterParameter(22, ShardUp);
        ui.RegisterParameter(23, SlowMo);
        ui.RegisterParameter(24, Hueness);
    }
	
	// Update is called once per frame
	void Update () {

        Mega.x = ui.GetParameterValue(21, Mega);
        ShardUp.x = ui.GetParameterValue(22, ShardUp);
        SlowMo.x = ui.GetParameterValue(23, SlowMo);
        Hueness.x = ui.GetParameterValue(24, Hueness);

        // Mega
        Shards.gravity = new Vector3(0.0f, Mathf.Lerp(-.4f, 6.0f, Mega.x), 0.0f);
        Shards.noiseAmplitude = Math.Lerp(.4f, 4.0f, Mega.x);
        Shards.speedLimit = Math.Lerp(.4f, 11.0f, Mega.x);
        ShardsMat.SetFloat("_SpeedToIntensity", Mathf.Lerp(.02f, .4f, Mega.x));
        ShardsMat.SetFloat("_Brightness", Mathf.Lerp(3.0f, 5.0f, Mega.x));

        // ShardUp
        Shards.speedToScale = Math.Lerp(.05f, .2f, ShardUp.x);
        ShardsMat.SetFloat("_EmissionProb", Math.Lerp(1.0f, 1.0f, ShardUp.x));

        // SlowMo
        Shards.drag = Math.Lerp(2.0f, 15.0f, SlowMo.x);
        Shards.maxLife = Math.Lerp(2.0f, 4.0f, SlowMo.x);

        // Hueness
        ShardsMat.SetFloat("_BaseHue", Hueness.x);
        TrailMat.SetFloat("_BaseHue", Hueness.x);



    }
}
