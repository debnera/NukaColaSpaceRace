using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;       //Public variable to store a reference to the player game object
    public float VelocityOffsetMultiplier = 0f;
    public float MinVelocity = 10f;
    public float ZoomSpeed = 100f;


    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
    private Rigidbody playerRigidbody;
    private float currentZoom;

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
//        offset = transform.position - player.transform.position;
        offset = transform.position - GetPlayerPosition();
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    Vector3 GetPlayerPosition()
    {
        return player.transform.position + player.transform.TransformVector(player.GetComponent<Rigidbody>().centerOfMass);
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        var zoomDirection = new Vector3(0, 0, -1);
        
        var targetZoom = Mathf.Max(playerRigidbody.velocity.magnitude - MinVelocity, 0) * VelocityOffsetMultiplier;
        if (currentZoom < targetZoom)
            currentZoom = Mathf.Min(currentZoom + ZoomSpeed * Time.deltaTime, targetZoom);
        else if (currentZoom > targetZoom)
            currentZoom = Mathf.Max(currentZoom - ZoomSpeed * Time.deltaTime, targetZoom);
        transform.position = GetPlayerPosition() + offset + zoomDirection * currentZoom;
    }
}
