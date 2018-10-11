using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashSound : MonoBehaviour
{

    public AudioClip crashSoft;
    public AudioClip crashHard;
    public AudioClip crashMed;

    private AudioSource source;
    private float lowPitchRange = .75F;
    private float highPitchRange = 1.25F;
    private float velToVol = .07F;
    private float velocityClipSplit = 5F;
    private float velocityClipSplitUp = 10F;

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

        if (coll.relativeVelocity.magnitude > velocityClipSplitUp)
            source.PlayOneShot(crashHard, hitVol);

        else
            source.PlayOneShot(crashMed, hitVol);
    }
}
