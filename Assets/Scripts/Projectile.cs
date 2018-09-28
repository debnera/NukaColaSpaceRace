using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float maxSecondsAlive = 10f;
    public ParticleSystem collisionParticleSystem;

	// Use this for initialization
	void Start ( )
	{
        // Set initial velocity
        GetComponent<Rigidbody>().velocity = speed * transform.up;
	    Destroy(gameObject, maxSecondsAlive);
	    if (collisionParticleSystem != null)
	    {
	        collisionParticleSystem.Stop();
	    }
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
        if (collisionParticleSystem != null)
        {
            ParticleSystem pSystem = Instantiate(collisionParticleSystem, transform.position, transform.rotation);
            pSystem.Play();
        }
        collision.gameObject.SendMessageUpwards("ApplyDamage", 1, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
