using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour {

    public GameObject ship;
    public float PayloadForce;
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

    [SerializeField] AudioClip[] shootingclips;

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

    private FixedJoint LeftJoint;
    private FixedJoint RightJoint;
    GameObject LeftPackage = null;
    GameObject RightPackage = null;

    private float originalMass;

    private GameManager gameManager;

    // Use this for initialization
    void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        originalMass = GetComponent<Rigidbody>().mass;
        
//        RightJoint = gameObject.AddComponent<FixedJoint>();
//        LeftJoint = gameObject.AddComponent<FixedJoint>();
        
    }
    
    void Start ( ) {
        rigidBody = GetComponent<Rigidbody>( );
        eulerAngleVelocity = new Vector3( 0, 0, RotationSpeed);
        
        previousShotTime = Time.time;
        gameManager = GameManager.GetInstance();
        shootingSound = ShootingSound.GetComponent<AudioSource>();
        destroyedSound = DestroyedSound.GetComponent<AudioSource>();
        landingSound = LandingSound.GetComponent<AudioSource>();
        cargoSound = CargoSound.GetComponent<AudioSource>();
        
//        var hpoint = GameObject.Find("HardPoint_Right");
//        RightJoint.anchor = hpoint.transform.localPosition;
//        hpoint = GameObject.Find("HardPoint_Left");
//        LeftJoint.anchor = hpoint.transform.localPosition;
    }

    void ResetPosition( )
    {
        IsLanded = false;
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
        if ( Input.GetKey(KeyCode.UpArrow) ) {
            rigidBody.AddForce(transform.up * Force * Time.fixedDeltaTime);
            EngineEffect.Play();
        }
            
	    // Fix rotation and z-position drift
	    var rot = transform.rotation.eulerAngles;
	    rot = new Vector3(0, 0, rot.z);
	    transform.rotation = Quaternion.Euler(rot);
	    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	    var pos = transform.position;
	    pos.z = initialPosition.z;
	    transform.position = pos;
	    
	    // Add gravity to payloads
	    if (LeftPackage)
	    {
	        var hpoint = GameObject.Find("HardPoint_Left");
	        var dir = hpoint.transform.InverseTransformVector(new Vector3(0, 1, 0));
	        GetComponent<Rigidbody>().AddForceAtPosition(dir * PayloadForce * Time.fixedDeltaTime, hpoint.transform.localPosition);
	    }
	    if (RightPackage)
	    {
	        var hpoint = GameObject.Find("HardPoint_Right");
	        var dir = hpoint.transform.InverseTransformVector(new Vector3(0, 1, 0));
	        GetComponent<Rigidbody>().AddForceAtPosition(dir * PayloadForce * Time.fixedDeltaTime, hpoint.transform.localPosition);
	    }
	    
	    
        // Check input
        if ( Input.GetKey( KeyCode.LeftArrow ) )
            TurnLeft( );

        if ( Input.GetKey( KeyCode.RightArrow ) )
            TurnRight( );

        if (Input.GetKey( KeyCode.Space))
            Fire();
	    
	    print(GetComponent<Rigidbody>().centerOfMass);
    }

    public void AttachRightPackage( GameObject package ) {
        if (RightPackage != null) return;
        cargoSound.Play();
        var hpoint = GameObject.Find("HardPoint_Right");
        
        var npackage = Instantiate(package, hpoint.transform);
        npackage.transform.localPosition = Vector3.zero;
        npackage.transform.localScale = Vector3.one;
//        var joint = hpoint.GetComponent<FixedJoint>();
//        if (!joint)
//            joint = hpoint.AddComponent<FixedJoint>();
//        joint.connectedBody = npackage.GetComponent<Rigidbody>();
//        npackage.GetComponent<Rigidbody>().isKinematic = false;
        package.SetActive(false);
        Destroy(package);
        RightPackage = npackage;
        GetComponent<Rigidbody>().ResetCenterOfMass();
    }

    public void AttachLeftPackage(GameObject package )
    {
        if (LeftPackage != null) return;
        cargoSound.Play();
        var hpoint = GameObject.Find("HardPoint_Left");
        var npackage = Instantiate(package, hpoint.transform);
        npackage.transform.localPosition = Vector3.zero;
        npackage.transform.localScale = Vector3.one;
//        var joint = hpoint.GetComponent<FixedJoint>();
//        if (!joint)
//            joint = hpoint.AddComponent<FixedJoint>();
//        joint.connectedBody = npackage.GetComponent<Rigidbody>();
//        npackage.GetComponent<Rigidbody>().isKinematic = false;
        package.SetActive(false);
        Destroy(package);
        LeftPackage = npackage;
        GetComponent<Rigidbody>().ResetCenterOfMass();
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
        if (!IsAlive) return;
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
            int index = UnityEngine.Random.Range(0, shootingclips.Length);
            shootingSound.clip = shootingclips[index];
            shootingSound.Play();
        }
    }

    private void TurnRight( )
    {
//        Quaternion deltaRotation = Quaternion.Euler(-eulerAngleVelocity * Time.fixedDeltaTime);
//        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
        
        GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, -RotationSpeed));
        //GetComponent<Rigidbody>().AddForceAtPosition(cannon.transform.right * RotationSpeed, cannon.transform.position, );
    }

    private void TurnLeft( )
    {
//        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.fixedDeltaTime);
//        rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
        GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, RotationSpeed));
        //GetComponent<Rigidbody>().AddForceAtPosition(-cannon.transform.right * RotationSpeed, cannon.transform.position);
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
