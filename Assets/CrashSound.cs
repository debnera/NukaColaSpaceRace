using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashSound : MonoBehaviour
{

    public AudioClip crashSoft;
    public AudioClip crashHard;

    private AudioSource source;
    private float lowPitchRange = .75F;
    private float highPitchRange = 1.25F;
    private float velToVol = .05F;
    private float velocityClipSplit = 5F;


    // Use this for initialization
    void Awake()
    {

        source = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision coll)
    {

        source.pitch = Random.Range(lowPitchRange, highPitchRange);
        float hitVol = coll.relativeVelocity.magnitude * velToVol;
        if (coll.relativeVelocity.magnitude < velocityClipSplit)
            source.PlayOneShot(crashSoft, hitVol);
        else
            source.PlayOneShot(crashHard, hitVol);
    }
}
