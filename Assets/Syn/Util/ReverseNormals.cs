using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ReverseNormals : MonoBehaviour {

    Mesh m;

	// Use this for initialization
	void Start () {
        m = GetComponent<MeshFilter>().mesh;

        Vector3[] normals = m.normals;

        for( int i=0; i<normals.Length; i++)
        {
            normals[i].x *= -1.0f;
            normals[i].y *= -1.0f;
            normals[i].z *= -1.0f;//= -1.0f * normals[i];
        }

        m.normals = normals;
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
