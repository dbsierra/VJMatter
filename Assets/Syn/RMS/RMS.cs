using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMS : MonoBehaviour {

    // Number of samples to use in our calculation of the instantaneous RMS
    private const int QSamples = 440;

    public AudioSource audioSource;
    float[] _samples;
    float RmsValue;

    // Running tab of the current frame number to use for averaging
    int currentFrame = 0;

    // How far back do we want to record RMS
    public int RMSHistorySize = 50;

    // How far back do we want to average out the RMS
    public int AvgPopulation = 25;

    // Material that contains the RMS History calculation shader
    public Material RMSMat;

    // Render textures to output our shader data to
    RenderTexture rtexRMSCurr;
    RenderTexture rtexRMSPrev;

    public float AvgRMS;

    void Start() {
        _samples = new float[QSamples];

        currentFrame = 0;

        // Create the Render Textures for our RMS data
        rtexRMSCurr = new RenderTexture(RMSHistorySize, RMSHistorySize, 0, RenderTextureFormat.ARGBFloat);
        rtexRMSCurr.filterMode = FilterMode.Point;
        rtexRMSCurr.wrapMode = TextureWrapMode.Clamp;
        rtexRMSPrev = new RenderTexture(RMSHistorySize, RMSHistorySize, 0, RenderTextureFormat.ARGBFloat);
        rtexRMSPrev.filterMode = FilterMode.Point;
        rtexRMSPrev.wrapMode = TextureWrapMode.Clamp;
        
        // Texture size set in shader
        RMSMat.SetFloat("_TexSize", RMSHistorySize);    
    }

    // Return capacity of RMS history buffer
    public int GetRMSHistorySize()
    {
        return RMSHistorySize;
    }

    // Return current amount filled in the RMS history buffer
    public int GetCurrentSize()
    {
        return (int)(Mathf.Min(currentFrame, RMSHistorySize));
    }

    // Return texture containing all the data
    public RenderTexture GetNoveltyHistoryTexture()
    {
        return rtexRMSCurr;
    }

    void Update()
    {
        // Obtain the current RMS of the playing audio
        AnalyzeRMS();

        RMSMat.SetFloat("_IncomingRMS", RmsValue);      // Incoming instantaneous RMS value
        RMSMat.SetFloat("_AverageSize", AvgPopulation); // How many many frames do we want to keep running average of
        RMSMat.SetFloat("_CurrentCount", currentFrame); // Current frame number, used for calculating running average

        // Calculate the AverageRMS and Novelty
        Graphics.Blit(rtexRMSPrev, rtexRMSCurr, RMSMat);
        Graphics.Blit(rtexRMSCurr, rtexRMSPrev);

        // CPU Avg RMS
        AvgRMS = (RmsValue + Mathf.Min(currentFrame, AvgPopulation) * AvgRMS) / (currentFrame + 1);

        currentFrame++;

       // Debug.Log(RMSMat.GetFloat("_TexSize"));
    }

    private void AnalyzeRMS()
    {
        // Fill array of size QSamples with samples
        audioSource.GetOutputData(_samples, 0);

        // Get the sum of all squared samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; 
        }

        // RMS = square root of average of squared samples
        RmsValue = Mathf.Sqrt(sum / QSamples); 
    }
    
    public float GetCurrentRMS()
    {
        return RmsValue;
    }

}
