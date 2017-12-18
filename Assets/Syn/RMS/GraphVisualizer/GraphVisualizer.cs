using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Syn.Particles;

public class GraphVisualizer : MonoBehaviour
{
    public Material GraphVisualizerMat;
    public Syn.Particles.ParticleSystem Particles;
    public RMS rmsObj;
    Texture2D testTex;

    [Range(0.0f, 1.0f)]
    public float R;
    [Range(0.0f, 1.0f)]
    public float G;
    [Range(0.0f, 1.0f)]
    public float B;

    // Use this for initialization
    void Awake()
    {
        testTex = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        testTex.filterMode = FilterMode.Point;
        Color[] BlackPixels = testTex.GetPixels();
        int i = 0;
        for (i = 0; i < BlackPixels.Length; i++)
        {
            BlackPixels[i] = Color.red;
        }
        testTex.SetPixels(BlackPixels);
        testTex.Apply();
    }
    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    { 

// GraphVisualizerMat.SetFloatArray("_Values2", rmsObj.GetRMSHistory());

    //    GraphVisualizerMat.SetFloatArray("_Values3", rmsObj.GetAverageHistory());

        GraphVisualizerMat.SetTexture("_Data", rmsObj.GetNoveltyHistoryTexture());
        GraphVisualizerMat.SetFloat("_R", R);
        GraphVisualizerMat.SetFloat("_G", G);
        GraphVisualizerMat.SetFloat("_B", B);

        GraphVisualizerMat.SetTexture("_VelocityTex", Particles.velTexCurr);

       // RenderTexture.active = rmsObj.GetRMSRenderTexture();
       // dataTex.ReadPixels(new Rect(0, 0, rmsObj.GetRMSRenderTexture().width, rmsObj.GetRMSRenderTexture().height), 0, 0);
       // dataTex.Apply();
       // Debug.Log(dataTex.GetPixel(0, 0));

        int i = rmsObj.GetCurrentSize()-1;
        if (i < 0)
            i = 0;
        GraphVisualizerMat.SetInt("_ValuesLength", i);

       // Debug.Log(rmsObj.GetRMSHistory()[i]);


        Graphics.Blit(src, dest, GraphVisualizerMat);
    }

}
