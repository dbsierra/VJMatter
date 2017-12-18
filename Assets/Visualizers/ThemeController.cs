using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kvant;
using MidiJack;
using Syn.Particles;
using UnityEngine.PostProcessing;

public class ThemeController : MonoBehaviour {

    int CurrentThemeIndex;

    public float ColorLerpTime = .1f;
    float colorTimerStart = 0.0f;
    bool animatingColor = false;
    Color orgCamColor;
    Color orgVignetteColor;
    Color orgLightColor;
    Color newCamColor;
    Color newVignetteColor;
    Color newLightColor;

    public Camera Cam;
    public Light SceneLight;
    public MeshRenderer HeroObject;
    public MeshRenderer DiscObject;
    PostProcessingProfile PostFX;
    VignetteModel.Settings vignetteSettings;

    public Theme[] SprayThemes;

    public Spray[] Neons;
    public Spray[] Opaques;
    public Spray[] CherryBlossoms;
    public Spray[] Coins;
    public Spray[] Bottles;

    public Syn.Particles.ParticleUI[] Pulsers;

    public bool ForceTheme1;
    public bool ForceTheme2;
    public bool ForceTheme3;
    public bool ForceTheme4;
    public bool ForceTheme5;
    public bool ForceTheme6;
    public bool ForceTheme7;
    public bool ForceTheme8;

    void Start ()
    {   
        if( Cam.gameObject.GetComponent<PostProcessingBehaviour>() != null)
        {
            if (Cam.gameObject.GetComponent<PostProcessingBehaviour>().profile != null)
            {
                PostFX = Cam.gameObject.GetComponent<PostProcessingBehaviour>().profile;
                vignetteSettings = PostFX.vignette.settings;
            }
        }
        PostFX.vignette.settings = vignetteSettings;

        NewTheme(0);
    }

    int currentTheme;

    void Update ()
    {
        if(MidiMaster.GetKeyDown(39))
        {
            currentTheme = (currentTheme+1)%5;
            NewTheme(currentTheme);
        }
        if (MidiMaster.GetKeyDown(9) || ForceTheme1)
        {
            ForceTheme1 = false;
            NewTheme(0);
        }
        else if(MidiMaster.GetKeyDown(10) || ForceTheme2)
        {
            ForceTheme2 = false;
            NewTheme(1);
        }
        else if (MidiMaster.GetKeyDown(11) || ForceTheme3)
        {
            ForceTheme3 = false;
            NewTheme(2);
        }
        else if (MidiMaster.GetKeyDown(12) || ForceTheme4)
        {
            ForceTheme4 = false;
            NewTheme(3);
        }
        else if (MidiMaster.GetKeyDown(25) || ForceTheme5)
        {
            ForceTheme5 = false;
            NewTheme(4);
        }
        /*
        else if (MidiMaster.GetKeyDown(26) || ForceTheme6)
        {
            ForceTheme6 = false;
            NewTheme(5);
        }
        else if (MidiMaster.GetKeyDown(27) || ForceTheme7)
        {
            ForceTheme7 = false;
            NewTheme(6);
        }
        else if (MidiMaster.GetKeyDown(28) || ForceTheme8)
        {
            ForceTheme8 = false;
            NewTheme(7);
        }*/

        if( animatingColor )
        {
            AnimateColor();
        }
    }

    private void NewTheme(int id)
    {
        CurrentThemeIndex = (int)Mathf.Clamp(id, 0, SprayThemes.Length - 1);
        colorTimerStart = Time.time;
        animatingColor = true;
        
        foreach (ParticleUI p in Pulsers)
        {
            p.NewTargetColor(SprayThemes[CurrentThemeIndex].gradient);
        }

        if(PostFX != null)
        {
            orgVignetteColor = PostFX.vignette.settings.color;
            newVignetteColor = SprayThemes[CurrentThemeIndex].vignetteColor;
           // vignetteSettings.color = SprayThemes[CurrentThemeIndex].vignetteColor;
           // PostFX.vignette.settings = vignetteSettings;
        }
        if(Cam != null)
        {
            orgCamColor = Cam.backgroundColor;
            newCamColor = SprayThemes[CurrentThemeIndex].backgroundColor;
          //  Cam.backgroundColor = SprayThemes[CurrentThemeIndex].backgroundColor;
        }
        if(SceneLight != null)
        {
            orgLightColor = SceneLight.color;
            newLightColor = SprayThemes[CurrentThemeIndex].lightColor;
        }
        if(HeroObject != null)
        {
            HeroObject.material = SprayThemes[CurrentThemeIndex].heroObject;
        }
        if(DiscObject != null)
        {
            DiscObject.material = SprayThemes[CurrentThemeIndex].discObject;
        }
        foreach(Spray s in Neons)
        {
            if (SprayThemes[CurrentThemeIndex].neon != null)
            {
                s.material = SprayThemes[CurrentThemeIndex].neon;
            }
        }
        foreach (Spray s in Opaques)
        {
            if (SprayThemes[CurrentThemeIndex].opaque != null)
            {
                s.material = SprayThemes[CurrentThemeIndex].opaque;
            }
        }
        foreach (Spray s in CherryBlossoms)
        {
            if (SprayThemes[CurrentThemeIndex].cherryBlossom != null)
            {
                s.material = SprayThemes[CurrentThemeIndex].cherryBlossom;
            }
        }
        foreach (Spray s in Coins)
        {
            if (SprayThemes[CurrentThemeIndex].coin != null)
            {
                s.material = SprayThemes[CurrentThemeIndex].coin;
            }
        }
        foreach (Spray s in Bottles)
        {
            if (SprayThemes[CurrentThemeIndex].bottle != null)
            {
                s.material = SprayThemes[CurrentThemeIndex].bottle;
            }
        }
    }

    private void AnimateColor()
    {
        if (ColorLerpTime > 0)
        {
            float timer = (Time.time - colorTimerStart) / ColorLerpTime;
            vignetteSettings.color = Color.Lerp(orgVignetteColor, newVignetteColor, timer);
            PostFX.vignette.settings = vignetteSettings;
            Cam.backgroundColor = Color.Lerp(orgCamColor, newCamColor, timer);
            SceneLight.color = Color.Lerp(orgLightColor, newLightColor, timer);
            RenderSettings.fogColor = Color.Lerp(orgCamColor, newCamColor, timer);

            if ( timer >= 1.0f )
            {
                animatingColor = false;
            }
        }
        else
        {
            vignetteSettings.color = newVignetteColor;
            PostFX.vignette.settings = vignetteSettings;
            Cam.backgroundColor =newCamColor;
            SceneLight.color = newLightColor;
            RenderSettings.fogColor = newCamColor;
        }
    }
}

[System.Serializable]
public class Theme
{
    public Material neon;
    public Material opaque;
    public Material cherryBlossom;
    public Material coin;
    public Material bottle;
    public Material heroObject;
    public Material discObject;

    public Texture2D gradient;

    public Color backgroundColor;
    public Color vignetteColor;
    public Color lightColor;

    public Theme()
    {

    }
}