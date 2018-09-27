using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payload : MonoBehaviour {

    public bool isLeftPackage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if (isLeftPackage)
            other.gameObject.SendMessage("AttachLeftPackage", gameObject,SendMessageOptions.DontRequireReceiver);
        else
            other.gameObject.SendMessage("AttachRightPackage", gameObject, SendMessageOptions.DontRequireReceiver);
    }
}
