using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwim : MonoBehaviour {

    public Material ColorSwimMat;

    public float Mix;
    [Range(0,1)]
    public float LogoMix;
    public float NoiseFreq;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        ColorSwimMat.SetFloat("_LogoMix", LogoMix);
        ColorSwimMat.SetFloat("_Mix", Mix);
        ColorSwimMat.SetFloat("_NoiseFreq", NoiseFreq);
        Graphics.Blit(src, dest, ColorSwimMat);
    }

}
