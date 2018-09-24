using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    public float maxSpeed = 1.0f;
    public float maxAcceleration = 0.1f;
    public float rateOfFire = 1.0f;
    public float degreesBetweenShots = 30.0f;
    public Rigidbody projectilePrefab;
    public GameObject movementEndPoint;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private float shootingAngle;
    private float previousShotTime;
    private float speed;

	// Use this for initialization
	void Start ()
	{
	    startPoint = transform.position;
	    endPoint = movementEndPoint.transform.position;
	    movementEndPoint.SetActive(false);
	    previousShotTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
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
	    if (Time.time - previousShotTime > (1 / rateOfFire))
	    {
	        previousShotTime = Time.time;
            Rigidbody projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, shootingAngle));
	        projectile.GetComponent<Projectile>().SetToIgnoreEnemyCollisions();  // Enemies cannot kill enemies
	        shootingAngle += degreesBetweenShots;
        }
    }
}
