using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadPlatform : MonoBehaviour
{
	public Payload payload;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	private void OnCollisionStay(Collision other)
	{
		if (payload)
		{
			bool success = payload.TryAttachPayload(other.gameObject);
			if (success) 
				payload = null;
		}
	}
}
