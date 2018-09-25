using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour {

    public GameObject ship;
    public float RotationSpeed = 4.0f;
    public float Force = 40.0f;
    private Rigidbody rigidBody;

    Vector3 eulerAngleVelocity;
    Collider objectCollider;
    Vector3 initialPosition;
    Quaternion initialRotation;

    // Use this for initialization
    void Start ( ) {
        rigidBody = GetComponent<Rigidbody>( );
        eulerAngleVelocity = new Vector3( 0, RotationSpeed, 0 );
        objectCollider = GetComponent<Collider>( );
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void ResetPosition( ) {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rigidBody.angularVelocity = new Vector3();
        rigidBody.velocity = new Vector3();
    }

	// Update is called once per frame
	void FixedUpdate ( ) {

        // Throttle
        if ( Input.GetKey(KeyCode.Space) )
            rigidBody.AddForce( transform.forward * Force * Time.fixedDeltaTime);

        if ( Input.GetKey( KeyCode.LeftArrow ) )
            TurnLeft( );

        if ( Input.GetKey( KeyCode.RightArrow ) )
            TurnRight( );

        //if ( Input.GetKey( KeyCode.UpArrow ) )
        //    Fire( );

    }

    private void OnCollisionEnter(Collision other)
    {
        ResetPosition( );
        Debug.Log("entered");
    }

    private void Fire( )
    {
        throw new NotImplementedException( );
    }

    private void TurnRight( )
    {
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.fixedDeltaTime);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    }

    private void TurnLeft( )
    {
        Quaternion deltaRotation = Quaternion.Euler(-eulerAngleVelocity * Time.fixedDeltaTime);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    }
}
