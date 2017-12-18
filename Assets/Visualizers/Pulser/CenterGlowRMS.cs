using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterGlowRMS : MonoBehaviour {

    public RMS TheRMS;
    float currRMS;
    public float mult = 1.0f;
    float tarScale = 0.0f;
    float curScale = 0.0f;
    float vel = 0.0f;
    public float timeToReach = .1f;

    Vector3 orgScale;

	// Use this for initialization
	void Start () {
        orgScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        currRMS = TheRMS.GetCurrentRMS() * mult;

        // curScale = Mathf.SmoothDamp(tarScale, currRMS, ref vel, timeToReach);

        transform.localScale = orgScale * TheRMS.AvgRMS;

    }
}
