using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour {

    public ParticleSystem destructionEffect;

    private Vector3 offset;
    ParticleSystem explosion;
    bool firstExplosion = true;

    // Use this for initialization
    void Start () {
		
	}

    private void OnCollisionEnter ()
    {
        if (!firstExplosion) return;

        firstExplosion = false;
        explosion = Instantiate(destructionEffect, transform);
        destructionEffect.Play();
    }

    // Update is called once per frame
    void FixedUpdate () {

    }

    /*
    private void OnDestroy()
    {
        if (explosion != null)
            Destroy(explosion);
    }
    */
}
