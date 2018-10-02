using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payload : MonoBehaviour {

    public bool isLeftPackage;
    public int reward;

	public bool TryAttachPayload(GameObject target)
	{
		SpaceShip player = target.GetComponent<SpaceShip>();
		if (player && player.IsLanded)
		{
			if (isLeftPackage)
				player.AttachLeftPackage(gameObject);
			else
				player.AttachRightPackage(gameObject);
			return true;
		}
		return false;
	}
}
