using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBurst : MonoBehaviour {

    public float speed;
    public AnimationCurve anim;
    private float timerStart;
    private bool animating;

	// Use this for initialization
	void Start () {
        timerStart = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

        transform.Rotate( new Vector3(0.0f, speed, 0.0f) );

        if(animating)
        {
           speed = anim.Evaluate(Time.time - timerStart);

        }

        //anim.Evaluate

    }

    public void StartAnimation()
    {
        timerStart = Time.time;
        animating = true;
    }
}
