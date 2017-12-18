using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kvant;

[RequireComponent(typeof(Spray))]
public class SprayParticleStartStop : MonoBehaviour {

    public float scaleMult = 1.0f;
    Spray SpraySystem;

	// Use this for initialization
	void Start () {
        SpraySystem = GetComponent<Spray>();
	}
	
	// Update is called once per frame
	void Update () {
        SpraySystem.scale *= scaleMult;
	}
}
