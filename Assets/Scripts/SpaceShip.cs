﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour {

    public GameObject ship;
    public float SecondsToRestartAfterDeath = 3f;
    public int shields = 1;
    public float RotationSpeed = 100.0f;
    public float Force = 2000.0f;
    private Rigidbody rigidBody;
    public Rigidbody projectilePrefab;
    public float projectileSpeed = 100;

    public GameObject ShootingSound;
    public GameObject DestroyedSound;
    public GameObject LandingSound;
    public GameObject CargoSound;
    private AudioSource shootingSound;
    private AudioSource destroyedSound;
    private AudioSource landingSound;
    private AudioSource cargoSound;

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
    public bool IsAlive = true;

    GameObject LeftPackage = null;
    GameObject RightPackage = null;

    private GameManager gameManager;

    // Use this for initialization
    void Start ( ) {
        rigidBody = GetComponent<Rigidbody>( );
        eulerAngleVelocity = new Vector3( 0, 0, RotationSpeed);
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        previousShotTime = Time.time;
        gameManager = GameManager.GetInstance();
        shootingSound = ShootingSound.GetComponent<AudioSource>();
        destroyedSound = DestroyedSound.GetComponent<AudioSource>();
        landingSound = LandingSound.GetComponent<AudioSource>();
        cargoSound = CargoSound.GetComponent<AudioSource>();
    }

    void ResetPosition( )
    {
        IsAlive = true;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rigidBody.angularVelocity = new Vector3( );
        rigidBody.velocity = new Vector3( );
    }

	// Update is called once per frame
	void FixedUpdate ( )
	{
	    if (!IsAlive) return;

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
        cargoSound.Play();
        var hpoint = GameObject.Find("HardPoint_Right");
        var npackage = Instantiate(package, hpoint.transform);
        npackage.transform.localPosition = Vector3.zero;
        npackage.transform.localScale = Vector3.one;
        package.SetActive(false);
        Destroy(package);
        RightPackage = npackage;
    }

    public void AttachLeftPackage(GameObject package )
    {
        if (LeftPackage != null) return;
        cargoSound.Play();
        var hpoint = GameObject.Find("HardPoint_Left");
        var npackage = Instantiate(package, hpoint.transform);
        print(npackage);
        npackage.transform.localPosition = Vector3.zero;
        npackage.transform.localScale = Vector3.one;
        package.SetActive(false);
        Destroy(package);
        LeftPackage = npackage;
    }

    private bool IsLandingOk() {
        var angle = initialRotation.eulerAngles.z - rigidBody.rotation.eulerAngles.z;
        return rigidBody.velocity.magnitude < MaxLandingSpeed && angle < MaxLandingAngle;
    }

    private bool IsLandingAngleOk()
    {
        var angle = initialRotation.eulerAngles.z - rigidBody.rotation.eulerAngles.z;
        return  angle < MaxLandingAngle;
    }

    private bool IsCollisionSafe(Collision other)
    {
        // Projectile hits are handled by separate code
        if (other.gameObject.GetComponent<Projectile>()) 
            return true;

        if (other.gameObject.GetComponent<HomePlatform>() ||
            other.gameObject.GetComponent<PayloadPlatform>())
        {
            return IsLandingAngleOk();
        }

        return false;
    }

    private void OnCollisionEnter( Collision other )
    {
        if (!IsAlive) return; // Dead player cannot cause damage or return payloads to base

        // Cause enough damage to kill anything on collision (Will not do anything to landing pads etc)
        other.gameObject.SendMessageUpwards("ApplyDamage", 1000, SendMessageOptions.DontRequireReceiver);

        // Handle unsafe collisions
        if (!IsCollisionSafe(other))
        {
            Debug.Log( "Landing Failed" );
            Die();
            return;
        }
        
    }

    private bool IsFirstLanding() {
        return (IsLanded == false && IsLandingOk());
    }

    private void OnCollisionStay(Collision other)
    {
        if (IsFirstLanding()) landingSound.Play();

        IsLanded = IsLandingOk();
        if ( other.gameObject.GetComponent<HomePlatform>() && IsLanded ) {
            Debug.Log( "Landing OK" );

            if (LeftPackage)
            {
                cargoSound.Play();
                int score = LeftPackage.GetComponent<Payload>().reward;
                gameManager.CollectCargo();
                gameManager.AddToScore(score);
                Destroy(LeftPackage);
                LeftPackage = null;
            }
            if (RightPackage)
            {
                cargoSound.Play();
                int score = RightPackage.GetComponent<Payload>().reward;
                gameManager.CollectCargo();
                gameManager.AddToScore(score);
                Destroy(RightPackage);
                RightPackage = null;
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        IsLanded = false;
    }

    private void Fire( )
    {
        // Handle shooting
        if (Time.time - previousShotTime > (1 / rateOfFire))
        {
            previousShotTime = Time.time;
            Rigidbody projectile = Instantiate(projectilePrefab, cannon.transform.position, cannon.transform.rotation );
            projectile.GetComponent<Projectile>( ).SetToIgnorePlayerCollisions();
            projectile.GetComponent<Projectile>().speed = projectileSpeed;
            shootingSound.Play();
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

    void ApplyDamage(int value)
    {
        shields -= value;
        if (shields <= 0 && IsAlive) Die();
    }

    void Die()
    {
        IsAlive = false;
        gameManager.ReducePlayerLives();
        Invoke("ResetPosition", SecondsToRestartAfterDeath);
        // TODO: Play death animation
        destroyedSound.Play();
    }
}
