using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRes : MonoBehaviour {

    Camera cam;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
      //  Screen.SetResolution(1280, 720, true);
      //  Application.targetFrameRate = 60;

    }

    // Update is called once per frame
    void Update () {
	}
}
