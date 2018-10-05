using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

    private AudioSource HitSource;

    void Awake()
    {
        HitSource = GetComponent<AudioSource>();
    }

    void ApplyDamage(int value)
    {
        HitSource.Play();
    }
}
