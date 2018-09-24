using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float speed = 10f;
    public float maxSecondsAlive = 10f;

	// Use this for initialization
	void Start ()
	{
        // Set initial velocity
	    GetComponent<Rigidbody>().velocity = speed * transform.right;
	    Destroy(gameObject, maxSecondsAlive);
    }

    public void SetToIgnorePlayerCollisions()
    {
        // Set the projectile to go through the player
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void SetToIgnoreEnemyCollisions()
    {
        // Set the projectile to go through all enemies
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");
        Destroy(gameObject);
    }
}
