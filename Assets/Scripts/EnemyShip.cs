using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    public float maxSpeed = 1.0f;
    public float maxAcceleration;
    public float rateOfFire = 1.0f;
    public float degreesBetweenShots = 30.0f;
    public Rigidbody projectilePrefab;
    public GameObject movementEndPoint;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private float shootingAngle;
    private float previousShotTime;
    private float startTime;
    private float movementLength;

	// Use this for initialization
	void Start ()
	{
	    startPoint = transform.position;
	    endPoint = movementEndPoint.transform.position;
	    movementEndPoint.SetActive(false);
        startTime = Time.time;
	    previousShotTime = Time.time;
        movementLength = Vector3.Distance(startPoint, endPoint);
	}
	
	// Update is called once per frame
	void Update () {
	    // Move towards target position
	    var distanceTraveled = (Time.time - startTime) * maxSpeed;
	    var fraction = distanceTraveled / movementLength;
	    transform.position = Vector3.Lerp(startPoint, endPoint, fraction);

	    // Change direction if necessary
	    if (fraction >= 1.0f)
	    {
	        startTime = Time.time;
	        var temp = startPoint;
	        startPoint = endPoint;
	        endPoint = temp;
	    }

        // Handle shooting
	    if (Time.time - previousShotTime > (1 / rateOfFire))
	    {
	        previousShotTime = Time.time;
            Rigidbody projectile = (Rigidbody)Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, shootingAngle));
	        projectile.GetComponent<Projectile>().SetToIgnoreEnemyCollisions();  // Enemies cannot kill enemies
	        shootingAngle += degreesBetweenShots;
        }
    }
}
