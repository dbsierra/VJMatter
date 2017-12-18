using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kvant;
using Syn.UI;
using MidiJack;

public class KSprayLauncher : MonoBehaviour {

    UI ui;
    public RotateMe RotateMeCam;
    public MeshRenderer NoiseSphere;
    public GameObject NosieSphereSize;
    public GameObject CenterDisc;
    public ColorSwim ColorSwimObj;
    public Animator CameraBurst;

    private Vector3 nsScale;
    private Vector3 discScale;

    [Header("Objects")]
    public Spray qPaper;
    public Spray wCherryBlossoms;
    public Spray eCoins;
    public Spray rGlowingOrbs;
    public Spray tTriangles;
    public Spray yVinyl;

    [Header("Geo Objects")]
    public Spray aCubes;
    public Spray sIcosahedron;
    public Spray dRubix;
    public Spray fBottles;
    public Spray gPyramids;
    public Spray hDodecahedron;
    public Spray jTorus;
    public Spray kTeapot;
    public Spray lTube;
    public Spray scTballbox;

    private List<Spray> AllSprays;

    [Header("Letters Hit")]
    public bool bQ;
    public bool bW;
    public bool bE;
    public bool bR;
    public bool bT;
    public bool bY;
    public bool bA;
    public bool bS;
    public bool bD;
    public bool bF;
    public bool bG;
    public bool bH;
    public bool bJ;
    public bool bK;
    public bool bL;
    public bool bSemiColon;

    [Header("Controls")]
    public bool MIDIEnabled;
    public Vector3 NoiseSphereSize;
    public Vector3 CenterDiscSize;
    public Vector3 Drag;
    public Vector3 CameraSpeed;
    public Vector3 CameraSpeedY;
    public Vector3 NoiseSphereAmp;
    public Vector3 NoiseSphereFreq;
    public Vector3 Speed;
    public Vector3 Mix;
    public Vector3 NoiseFreq;


    // Use this for initialization
    void Start () {
        ui = new UI();
        ui.RegisterParameter(25, Mix);
        ui.RegisterParameter(26, NoiseFreq);

        ui.RegisterParameter(41, NoiseSphereSize);
        ui.RegisterParameter(42, CenterDiscSize);
        ui.RegisterParameter(43, NoiseSphereAmp);
        ui.RegisterParameter(44, NoiseSphereFreq);

        //   ui.RegisterParameter(47, Drag);
        ui.RegisterParameter(7, CameraSpeed);
        ui.RegisterParameter(8, CameraSpeedY);

        ui.RegisterParameter(6, Speed);

        nsScale = new Vector3(NoiseSphereSize.x, NoiseSphereSize.x, NoiseSphereSize.x);
        discScale = new Vector3(CenterDiscSize.x, CenterDiscSize.x, CenterDiscSize.x);

        AllSprays = new List<Spray>();
        AllSprays.Add(qPaper);
        AllSprays.Add(wCherryBlossoms);
        AllSprays.Add(eCoins);
        AllSprays.Add(rGlowingOrbs);
        AllSprays.Add(tTriangles);
        AllSprays.Add(yVinyl);
        AllSprays.Add(aCubes);
        AllSprays.Add(sIcosahedron);
        AllSprays.Add(dRubix);
        AllSprays.Add(fBottles);
        AllSprays.Add(gPyramids);
        AllSprays.Add(hDodecahedron);
        AllSprays.Add(jTorus);
        AllSprays.Add(kTeapot);
        AllSprays.Add(lTube);
        AllSprays.Add(scTballbox);
}

private void ApplyParameters()
{
        /*
        qPaper.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        wCherryBlossoms.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        eCoins.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        rGlowingOrbs.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        tTriangles.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        yVinyl.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        aCubes.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        sIcosahedron.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        dRubix.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        fBottles.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        gPyramids.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        hDodecahedron.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        jTorus.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        kTeapot.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        lTube.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        scTballbox.accelerationAdder = new Vector3(NoiseSphereSize.x, CenterDiscSize.x, 0.0f);
        */

        qPaper.accelerationMult = Speed.x;
        wCherryBlossoms.accelerationMult = Speed.x;
        eCoins.accelerationMult = Speed.x;
        rGlowingOrbs.accelerationMult =Speed.x;
        tTriangles.accelerationMult =Speed.x;
        yVinyl.accelerationMult =Speed.x;
        aCubes.accelerationMult =Speed.x;
        sIcosahedron.accelerationMult =Speed.x;
        dRubix.accelerationMult =Speed.x;
        fBottles.accelerationMult =Speed.x;
        gPyramids.accelerationMult =Speed.x;
        hDodecahedron.accelerationMult =Speed.x;
        jTorus.accelerationMult =Speed.x;
        kTeapot.accelerationMult =Speed.x;
        lTube.accelerationMult =Speed.x;
        scTballbox.accelerationMult =Speed.x;

      //  fBottles.life = Syn.Math.Map(Speed.x, Speed.y, Speed.z, 35, 15);

        qPaper.dragMult = Drag.x;
        wCherryBlossoms.dragMult = Drag.x;
        eCoins.dragMult = Drag.x;
        rGlowingOrbs.dragMult = Drag.x;
        tTriangles.dragMult = Drag.x;
        yVinyl.dragMult = Drag.x;
        aCubes.dragMult = Drag.x;
        sIcosahedron.dragMult = Drag.x;
        dRubix.dragMult = Drag.x;
        fBottles.dragMult = Drag.x;
        gPyramids.dragMult = Drag.x;
        hDodecahedron.dragMult = Drag.x;
        jTorus.dragMult = Drag.x;
        kTeapot.dragMult = Drag.x;
        lTube.dragMult = Drag.x;
        scTballbox.dragMult = Drag.x;
    }

	// Update is called once per frame
	void Update () {

        if(MidiMaster.GetKeyDown(26))
        {
            CameraBurst.SetTrigger("Burst");
        }

        if (MIDIEnabled)
        {
            Mix.x = ui.GetParameterValue(25, Mix);
            NoiseFreq.x = ui.GetParameterValue(26, NoiseFreq);

            NoiseSphereSize.x = ui.GetParameterValue(41, NoiseSphereSize);
            CenterDiscSize.x = ui.GetParameterValue(42, CenterDiscSize);
         //   Drag.x = ui.GetParameterValue(47, Drag);
            NoiseSphereAmp.x = ui.GetParameterValue(43, NoiseSphereAmp);
            NoiseSphereFreq.x = ui.GetParameterValue(44, NoiseSphereFreq);
            CameraSpeed.x = ui.GetParameterValue(7, CameraSpeed);
            CameraSpeedY.x = ui.GetParameterValue(8, CameraSpeedY);

            Speed.x = ui.GetParameterValue(6, Speed); 

        }

        ApplyParameters();

        if ( ColorSwimObj != null)
        {
            ColorSwimObj.Mix = Mix.x;
            ColorSwimObj.NoiseFreq = NoiseFreq.x;
        }

        if ( NoiseSphere != null)
        {
            NoiseSphere.material.SetFloat("_Amp", NoiseSphereAmp.x);
            NoiseSphere.material.SetFloat("_Freq", NoiseSphereFreq.x);

            nsScale.x = NoiseSphereSize.x;
            nsScale.y = NoiseSphereSize.x;
            nsScale.z = NoiseSphereSize.x;
            NosieSphereSize.transform.localScale = nsScale;
        }

        if( CenterDisc != null )
        {
            discScale.x = CenterDiscSize.x;
            discScale.y = CenterDiscSize.x;
            discScale.z = CenterDiscSize.x;
            CenterDisc.transform.localScale = discScale;
        }

        if (RotateMeCam != null)
        {
            RotateMeCam.SpeedX = CameraSpeed.x;
            RotateMeCam.SpeedY = CameraSpeedY.x;

        }

        #region phase 1
        if ( Input.GetKeyDown(KeyCode.Q) || MidiMaster.GetKeyDown(40) )
        {
            TriggerParticles(qPaper);
            bQ = !bQ;
        }
        if (Input.GetKeyDown(KeyCode.W) || MidiMaster.GetKeyDown(41) )
        {
            TriggerParticles(wCherryBlossoms);
            bW = !bW;
        }
        if (Input.GetKeyDown(KeyCode.E) || MidiMaster.GetKeyDown(50))
        {
            TriggerParticles(eCoins);
            bE = !bE;
        }
        if (Input.GetKeyDown(KeyCode.R) || MidiMaster.GetKeyDown(51))
        {
            TriggerParticles(rGlowingOrbs);
            bR = !bR;
        }
        if (Input.GetKeyDown(KeyCode.T) || MidiMaster.GetKeyDown(42))
        {
            TriggerParticles(tTriangles);
            bT = !bT;
        }
        if (Input.GetKeyDown(KeyCode.Y) || MidiMaster.GetKeyDown(53))
        {
            TriggerParticles(yVinyl);
            bY = !bY;
        }
#endregion

        if (Input.GetKeyDown(KeyCode.A) || MidiMaster.GetKeyDown(54))
        {
            TriggerParticles(aCubes);
            bA = !bA;
        }
        if (Input.GetKeyDown(KeyCode.S) || MidiMaster.GetKeyDown(55))
        {
            TriggerParticles(sIcosahedron);
            bS = !bS;
        }
        if (Input.GetKeyDown(KeyCode.D) || MidiMaster.GetKeyDown(56))
        {
            TriggerParticles(dRubix);
            bD = !bD;
        }
        if (Input.GetKeyDown(KeyCode.F) || MidiMaster.GetKeyDown(43))
        {
            TriggerParticles(fBottles);
            bF = !bF;
        }
        if (Input.GetKeyDown(KeyCode.G) || MidiMaster.GetKeyDown(36))
        {
            TriggerParticles(gPyramids);
            bG = !bG;
        }
        if (Input.GetKeyDown(KeyCode.H) || MidiMaster.GetKeyDown(59))
        {
            TriggerParticles(hDodecahedron);
            bH = !bH;
        }
        if (Input.GetKeyDown(KeyCode.J) || MidiMaster.GetKeyDown(60))
        {
            TriggerParticles(jTorus);
            bJ = !bJ;
        }
        if (Input.GetKeyDown(KeyCode.K) || MidiMaster.GetKeyDown(37))
        {
            TriggerParticles(kTeapot);
            bK = !bK;
        }
        if (Input.GetKeyDown(KeyCode.L) || MidiMaster.GetKeyDown(62))
        {
            TriggerParticles(lTube);
            bL = !bL;
        }
        if (Input.GetKeyDown(KeyCode.Semicolon) || MidiMaster.GetKeyDown(63))
        {
            TriggerParticles(scTballbox);
            bSemiColon = !bSemiColon;
        }

        if( Input.GetKeyDown(KeyCode.Z) || MidiMaster.GetKeyDown(64))
        {
            KillAll();
        }


    }

    private void KillAll()
    {
        foreach(Spray ps in AllSprays)
        {
            Animator ac = ps.gameObject.GetComponent<Animator>();
            if(ac.GetCurrentAnimatorStateInfo(0).IsName("Start"))
            {
                ac.SetTrigger("Go");
            }

        }
    }

    private void TriggerParticles(Spray ps)
    {
        Animator ac = ps.gameObject.GetComponent<Animator>();

        //Debug.Log(ac.GetCurrentAnimatorStateInfo(0).IsName("Stop"));

        ac.SetTrigger("Go");
    }
}

[System.Serializable]
public class SpraySlot
{
    public Spray System;

    public enum SprayMeshS
    {
        cube=0,
        triangle=1
    };

    public SprayMeshS SprayMesh;


    public SpraySlot()
    {

    }
}
