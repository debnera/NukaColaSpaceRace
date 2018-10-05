using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    public ParticleSystem deathParticleSystem;
    public int shields;
    public int reward;
    public float maxSpeed = 1.0f;
    public float maxAcceleration = 0.1f;
    public float rateOfFire = 1.0f;
    //public float degreesBetweenShots = 30.0f;
    public float detectionRange = 50;
    public float maxInaccuracyAngle = 10.0f;
    public Rigidbody projectilePrefab;
    public GameObject movementEndPoint;

    public GameObject FlyingSound;
    public GameObject ShootingSound;
    public GameObject HitSound;
    public GameObject DestroyedSound;

    private AudioSource FlyingSource;
    private AudioSource ShootingSource;
    private AudioSource HitSource;
    private AudioSource DestroyedSource;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private float shootingAngle;
    private float previousShotTime;
    private float speed;
    private bool alive = true;
    private GameObject player;

    private void Awake()
    {
        FlyingSource = FlyingSound.GetComponent<AudioSource>();
        ShootingSource = ShootingSound.GetComponent<AudioSource>();
        HitSource = HitSound.GetComponent<AudioSource>();
        DestroyedSource = DestroyedSound.GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start( )
	{
	    startPoint = transform.position;
	    endPoint = movementEndPoint.transform.position;
	    movementEndPoint.SetActive(false);
	    previousShotTime = Time.time;
	    player = FindObjectOfType<SpaceShip>().gameObject;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    if (!alive) return;

	    // Move towards target position
	    speed = Mathf.Clamp(speed + maxAcceleration * Time.fixedDeltaTime, -maxSpeed, maxSpeed);
	    transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.fixedDeltaTime);

	    // Change direction if necessary
	    if (Vector3.Distance(transform.position, endPoint) < 0.3f)
	    {
	        speed = -speed;
	        var temp = startPoint;
	        startPoint = endPoint;
	        endPoint = temp;
	    }

        // Handle shooting
	    if (Time.time - previousShotTime > (1 / rateOfFire) && IsPlayerInLineOfSight())
	    {
	        previousShotTime = Time.time;
            //Rigidbody projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, shootingAngle));
	        Rigidbody projectile = Instantiate(projectilePrefab, transform.position, GetShootingQuaternion());
            projectile.GetComponent<Projectile>().SetToIgnoreEnemyCollisions();  // Enemies cannot kill enemies
            ShootingSource.Play();
        }
    }

    bool IsPlayerInLineOfSight()
    {
        // Returns true if player is within range and is on line of sight
        RaycastHit hit;
        int layerMask = ~LayerMask.NameToLayer("Enemy"); // Ignore collisions with enemies
        Debug.DrawRay(transform.position, GetVectorToPlayer() * detectionRange, Color.yellow);
        if (Physics.Raycast(transform.position, GetVectorToPlayer(), out hit, detectionRange, layerMask))
        {
            return hit.collider.gameObject == player;
        }
        return false;
    }

    Vector3 GetVectorToPlayer()
    {
        return ((player.GetComponent<Rigidbody>().centerOfMass + player.transform.position) - transform.position).normalized;
    }

    Quaternion GetShootingQuaternion()
    {
        Vector3 dir = GetVectorToPlayer();
        var angle = Vector3.Angle(transform.up, dir);
        angle += Random.Range(-maxInaccuracyAngle, maxInaccuracyAngle);
        return Quaternion.Euler(0, 0, angle);
    }

    void ApplyDamage(int value)
    {
        shields -= value;

        HitSource.Play();
        if (shields <= 0 && alive)
        {
            Die();
        }
    }

    void Die()
    {
        alive = false;
        if (deathParticleSystem != null)
        {
            ParticleSystem pSystem = Instantiate(deathParticleSystem, transform.position, transform.rotation);
            pSystem.Play();
        }
        FlyingSource.Stop();
        DestroyedSource.Play();
        FindObjectOfType<GameManager>().AddToScore(reward);
        Destroy(gameObject, DestroyedSource.clip.length);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Cause enough damage to kill anything on collision
        collision.gameObject.SendMessageUpwards("ApplyDamage", 1000, SendMessageOptions.DontRequireReceiver);
    }
}
