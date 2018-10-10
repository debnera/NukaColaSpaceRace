using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{

    public float timeOn;
    public float timeOff;
    public float timeStart;
    public bool active;
    public float radius;
    public float strength;
    public bool visualizeRadius;

    AudioSource magnetSound;

    private GameObject visualizer;

    // Use this for initialization
    void Start () {
        magnetSound = GetComponent<AudioSource>();
        if (visualizeRadius)
        {
            AddVisualization();
        }
        Invoke("Toggle", timeStart);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        if (!active) return;

        // Add force to all Rigidbodies in given radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var collider in hitColliders)
        {
            var rBody = collider.gameObject.GetComponent<Rigidbody>();
            if (rBody)
            {
                // Determine direction as a unit vector towards the magnet
                Vector3 direction = transform.position - rBody.transform.position;

                // Scale direction according to the strength of the magnet
                direction = direction.normalized * strength * Time.fixedDeltaTime;
                rBody.AddForce(direction);
            }
        }
    }

    void Toggle()
    {
        // Periodically toggles the magnet on/off 
        active = !active;
        
        if (active)
        {
            SetParticleSystems(true);
            magnetSound.Play();
            //GetComponent<MeshRenderer>().material.color = Color.green;
            SetVisualizerColor(Color.green);
            Invoke("Toggle", timeOn);
        }
        else
        {
            SetParticleSystems(false);
            magnetSound.Stop();
            //GetComponent<MeshRenderer>().material.color = Color.gray;
            SetVisualizerColor(Color.gray);
            Invoke("Toggle", timeOff);
        }
    }

    void SetParticleSystems(bool enabled)
    {
        var pSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var sys in pSystems)
        {
            if (enabled) sys.Play();
            else sys.Stop();
        }
    }

    void AddVisualization()
    {
        
        
        // Add a transparent sphere for visualizing the radius (Used for debugging / level design)
        visualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualizer.transform.position = transform.position;

        // Switch to shader that supports transparency
        visualizer.GetComponent<MeshRenderer>().material.shader = Shader.Find("Transparent/Diffuse");

        // Disable physics
        DestroyImmediate(visualizer.GetComponent<Rigidbody>());
        DestroyImmediate(visualizer.GetComponent<SphereCollider>());

        // Scale to hopefully correct size
        visualizer.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
    }

    void SetVisualizerColor(Color color)
    {
        if (!visualizeRadius) return;
        
        color.a = 0.1f; // Make the color transparent
        visualizer.GetComponent<MeshRenderer>().material.color = color;
    }
}
