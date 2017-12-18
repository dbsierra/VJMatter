using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DelayRGB : MonoBehaviour {

    public RenderTexture rt;
    public Material mat;

    [Range(0, 50)]
    public int DelayR;
    [Range(0, 50)]
    public int DelayG;
    [Range(0, 50)]
    public int DelayB;
    [Range(0, 1)]
    public float OriginalBrightness;

  //  Texture2DArray stream;
    private int readIndex;
    private int readIndexR;
    private int readIndexG;
    private int readIndexB;
    private int writeIndex;
    private int delayFrame;
    private int currFrame;
    private bool beginDisplay;
    RenderTexture renderStream;
    RenderTexture incomingFrame;
    private bool setFormat;

	void Start () {
        readIndex = writeIndex = 0;
        delayFrame = 50;
        currFrame = 1;

        incomingFrame = new RenderTexture(1280, 720, 1, RenderTextureFormat.ARGBHalf);
        incomingFrame.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        incomingFrame.useMipMap = true;
        incomingFrame.Create();

        renderStream = new RenderTexture(1280, 720, delayFrame, RenderTextureFormat.ARGB32);
        renderStream.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
        renderStream.useMipMap = true;
        renderStream.volumeDepth = delayFrame;
        renderStream.Create();

        mat.SetTexture("_MyArr", renderStream);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!setFormat)
        {
            renderStream.format = source.format;
            setFormat = false;
        }

        if (currFrame >= DelayR)
        {
            readIndexR = (currFrame - DelayR) % delayFrame;
        }
        if (currFrame >= DelayG)
        {
            readIndexG = (currFrame - DelayG) % delayFrame;
        }
        if (currFrame >= DelayB)
        {
            readIndexB = (currFrame - DelayB) % delayFrame;
        }
        writeIndex = (currFrame-1) % delayFrame;

        mat.SetFloat("_ReadIndexR", readIndexR);
        mat.SetFloat("_ReadIndexG", readIndexG);
        mat.SetFloat("_ReadIndexB", readIndexB);

        Graphics.CopyTexture(source, 0, 0, renderStream, writeIndex, 0);

        //   RenderTexture.active = source;
        //   cpuTex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        //   cpuTex.Apply();
        // cpuTexStream[writeIndex].LoadRawTextureData(cpuTex.GetRawTextureData());
        // cpuTexStream[writeIndex].Apply();
        //  RenderTexture.active = null;

        //  Debug.Log("write: " + writeIndex + "  read: " + readIndex + " " + beginDisplay);

        //   Debug.Log(source.width + " " + source.height);

        currFrame++;

        Graphics.Blit(source, destination, mat);
    }
}
