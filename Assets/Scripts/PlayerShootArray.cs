using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootArray : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] shoot;
    private AudioClip shootClip;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            int index = Random.Range(0, 7);
            shootClip = shoot[index];
            audioSource.clip = shootClip;
            audioSource.Play();
        }
    }
}
