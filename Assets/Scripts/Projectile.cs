using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float maxSecondsAlive = 10f;
    public ParticleSystem collisionParticleSystem;
    public AudioSource groundHitAudioSource;

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

    bool IsWall(GameObject obj)
    {
        // Hackish way of determining if the projectile hit a wall
        while (obj.transform.parent)
        {
            obj = obj.transform.parent.gameObject;
        }
        var cname = obj.name.ToLower();
        return cname.Contains("caver"); // Some walls are misspelled as "caver" instead of "cavern"
    }

    void OnCollisionEnter(Collision collision)
    {
        var go = new GameObject();
        Destroy(go, 5f);
        go.transform.position = transform.position;
        if (IsWall(collision.gameObject) && groundHitAudioSource)
        {
            AudioSource audio = Instantiate(groundHitAudioSource, go.transform);
            audio.Play();
        }

        if (collisionParticleSystem != null)
        {
            ParticleSystem pSystem = Instantiate(collisionParticleSystem, go.transform);
            pSystem.Play();
        }
        collision.gameObject.SendMessageUpwards("ApplyDamage", 1, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
