using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour {

    public GameObject ship;
    public float RotationSpeed = 4.0f;
    public float Force = 40.0f;
    private Rigidbody rigidBody;
    public Rigidbody projectilePrefab;
    public float projectileSpeed = 100;

    [SerializeField] GameObject cannon;

    Vector3 eulerAngleVelocity;
    Vector3 initialPosition;
    Quaternion initialRotation;

    private float previousShotTime;
    public float rateOfFire = 1.0f;

    // Use this for initialization
    void Start ( ) {
        rigidBody = GetComponent<Rigidbody>( );
        eulerAngleVelocity = new Vector3( 0, RotationSpeed, 0 );
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        previousShotTime = Time.time;
    }

    void ResetPosition( ) {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rigidBody.angularVelocity = new Vector3();
        rigidBody.velocity = new Vector3();
    }

	// Update is called once per frame
	void FixedUpdate ( ) {
        //update camera position
        //mainCamera.transform.position.Set(transform.position.x, transform.position.y, mainCamera.transform.position.z);

        // Throttle
        if ( Input.GetKey(KeyCode.Space) )
            rigidBody.AddForce( transform.forward * Force * Time.fixedDeltaTime);

        if ( Input.GetKey( KeyCode.LeftArrow ) )
            TurnLeft( );

        if ( Input.GetKey( KeyCode.RightArrow ) )
            TurnRight( );

        if (Input.GetKey(KeyCode.UpArrow))
            Fire();
    }

    private void OnCollisionEnter(Collision other)
    {
        ResetPosition( );
        Debug.Log( "entered" );
    }

    private void Fire( )
    {
        // Handle shooting
        if (Time.time - previousShotTime > (1 / rateOfFire))
        {
            previousShotTime = Time.time;
            Rigidbody projectile = Instantiate(projectilePrefab, cannon.transform.position, cannon.transform.rotation );
            projectile.GetComponent<Projectile>( ).SetToIgnorePlayerCollisions();
            //projectile.GetComponent<Projectile>().SetVelocity( projectileSpeed * cannon.transform.forward);
        }
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
