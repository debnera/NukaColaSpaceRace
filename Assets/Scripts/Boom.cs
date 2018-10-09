using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour {

    public ParticleSystem destructionEffect;

    private Vector3 offset;

    // Use this for initialization
    void Start () {
		
	}

    private void OnCollisionEnter ()
    {
        Instantiate(destructionEffect, transform.position, transform.rotation);
        destructionEffect.Play();

    }

    // Update is called once per frame
    void FixedUpdate () {

    }
}
