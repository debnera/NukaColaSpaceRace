using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootAudio : MonoBehaviour
{


    private AudioSource audioSource;
    public AudioClip playerShoot3;


   


    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           audioSource.PlayOneShot(playerShoot3);
        }
    }
}
