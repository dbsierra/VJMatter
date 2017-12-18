using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMe : MonoBehaviour {

    public float Speed;

    public float SpeedX;
    public float SpeedY;

    public float RotateMore = 1.0f;

 //   public Vector3 axis;

    float r1, r2, r3;

	// Use this for initialization
	void Start () {
        r1 = Random.Range(.5f, 2.5f);
        r2 = Random.Range(.45f, 2.75f);
        r3 = Random.Range(.65f, 2.45f);
    }
	
	// Update is called once per frame
	void Update () {

       // transform.localRotation = Quaternion.AngleAxis(Time.time * Speed * r1, Vector3.right) * Quaternion.AngleAxis(Time.time * Speed * r2, Vector3.up) * Quaternion.AngleAxis(Time.time * Speed * r3, Vector3.forward); ;

      //  transform.Rotate(new Vector3(0.0f, Mathf.Cos(Time.time) * .5f + .5f , 0.0f) * Speed);
      //  transform.Rotate(new Vector3(Mathf.Sin(Time.time*.9f) * .5f + .5f , 0.0f, 0.0f) * Speed);


        transform.Rotate(new Vector3(SpeedX, SpeedY, 0.0f));

        //
        //transform.Rotate(axis, Time.time * Speed);
    }


}
