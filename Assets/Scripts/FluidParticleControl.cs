using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidParticleControl : MonoBehaviour {

    public GameObject beaker;
    ParticleSystem particleSys;

    void Start () {
        particleSys = GetComponent<ParticleSystem>();
        particleSys.Stop();
	}
	
	void FixedUpdate () {
        
        float tiltAngle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(Vector3.up, beaker.transform.up) / (Vector3.up.sqrMagnitude * beaker.transform.up.sqrMagnitude));

        if (tiltAngle > 90 && tiltAngle < 270)
            particleSys.Play();
        else
            particleSys.Stop();
	}
}
