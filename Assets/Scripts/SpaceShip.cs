using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour {

    public GameObject ship;
    private Rigidbody rigidBody;
    // Use this for initialization
    void Start ( ) {
        rigidBody = GetComponent<Rigidbody>( );
    }
	
	// Update is called once per frame
	void Update ( ) {
        if( Input.GetKey( KeyCode.UpArrow ) ) {
            Vector3 start = ship.gameObject.transform.position;
            Vector3 end = start + new Vector3( 0, 0, 1.0f );
            rigidBody.MovePosition(end);
        }
	}
}
