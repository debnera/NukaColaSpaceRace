using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class thrustAudio : MonoBehaviour {


    public AudioMixerSnapshot thrustOn;
    public AudioMixerSnapshot thrustOff;

    public AudioMixer rocketCutoff;

    private float m_TransitionIn;
    private float m_TransitionOut;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Space))
            thrustOn.TransitionTo(0.001f);

        if (Input.GetKey(KeyCode.Space))
            rocketCutoff.SetFloat("rocketCutoff", 5000);
    

        else thrustOff.TransitionTo(0.35f);
        
    }
}
