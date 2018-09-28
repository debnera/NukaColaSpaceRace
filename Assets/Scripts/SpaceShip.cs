using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour {

    public GameObject ship;
    public float RotationSpeed = 100.0f;
    public float Force = 2000.0f;
    private Rigidbody rigidBody;
    public Rigidbody projectilePrefab;
    public float projectileSpeed = 100;

    public float MaxLandingAngle = 10.0f;
    public float MaxLandingSpeed = 3.0f;

    [SerializeField] GameObject cannon;
    [SerializeField] ParticleSystem EngineEffect;

    Vector3 eulerAngleVelocity;
    Vector3 initialPosition;
    Quaternion initialRotation;

    private float previousShotTime;
    public float rateOfFire = 1.0f;

    public bool IsLanded = true;

    GameObject LeftPackage = null;
    GameObject RightPackage = null;

    // Use this for initialization
    void Start ( ) {
        rigidBody = GetComponent<Rigidbody>( );
        eulerAngleVelocity = new Vector3( 0, 0, RotationSpeed);
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        previousShotTime = Time.time;

    }

    void ResetPosition( ) {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rigidBody.angularVelocity = new Vector3( );
        rigidBody.velocity = new Vector3( );
    }

	// Update is called once per frame
	void FixedUpdate ( ) {
        //update camera position
        //mainCamera.transform.position.Set(transform.position.x, transform.position.y, mainCamera.transform.position.z);

        // Throttle
        if ( Input.GetKey(KeyCode.Space) ) {
            rigidBody.AddForce(transform.up * Force * Time.fixedDeltaTime);
            EngineEffect.Play();
        }
            

        if ( Input.GetKey( KeyCode.LeftArrow ) )
            TurnLeft( );

        if ( Input.GetKey( KeyCode.RightArrow ) )
            TurnRight( );

        if (Input.GetKey( KeyCode.UpArrow ))
            Fire();
    }

    public void AttachRightPackage( GameObject package ) {
        if (RightPackage != null) return;
        var hpoint = GameObject.Find("HardPoint_Right");
        package.transform.position = Vector3.zero;
        //package.transform.rotation = ;
        var npackage = Instantiate(package, hpoint.transform);
        package.SetActive(false);
        Destroy(package);
        RightPackage = npackage;
    }

    public void AttachLeftPackage(GameObject package )
    {
        if (LeftPackage != null) return;
        var hpoint = GameObject.Find("HardPoint_Left");
        package.transform.position = Vector3.zero;
        //package.transform.rotation = ;
        var npackage = Instantiate(package, hpoint.transform);
        package.SetActive(false);
        Destroy(package);
        LeftPackage = npackage;
    }

    private bool IsLandingOk() {
        var angle = initialRotation.eulerAngles.z - rigidBody.rotation.eulerAngles.z;
        return rigidBody.velocity.magnitude < MaxLandingSpeed && angle < MaxLandingAngle;
    }

    private void OnCollisionEnter( Collision other )
    {
        if ( other.gameObject.name == "Platform" && IsLandingOk( ) ) {
            Debug.Log( "Landing OK" );
            //ResetPosition( );
        } else {
            Debug.Log( "Landing Failed" );
            //ResetPosition( );
        }
    }

    private void Fire( )
    {
        // Handle shooting
        if (Time.time - previousShotTime > (1 / rateOfFire))
        {
            previousShotTime = Time.time;
            Rigidbody projectile = Instantiate(projectilePrefab, cannon.transform.position, cannon.transform.rotation );
            projectile.GetComponent<Projectile>( ).SetToIgnorePlayerCollisions();
        }
    }

    private void TurnRight( )
    {
        Quaternion deltaRotation = Quaternion.Euler(-eulerAngleVelocity * Time.fixedDeltaTime);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    }

    private void TurnLeft( )
    {
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.fixedDeltaTime);
        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
    }
}
