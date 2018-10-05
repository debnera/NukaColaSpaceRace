using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payload : MonoBehaviour {

    public bool isLeftPackage;
    public int reward;
    private PayloadPlatform platform;

	public bool TryAttachPayload(GameObject target, PayloadPlatform platform)
	{
	    SetOwnerPlatform(platform); // This could be set from the editor, but I didn't want to touch the scenes
        SpaceShip player = target.GetComponent<SpaceShip>();
		if (player && player.IsLanded && player.IsAlive)
		{
			if (isLeftPackage)
				player.AttachLeftPackage(gameObject);
			else
				player.AttachRightPackage(gameObject);
			return true;
		}
		return false;
	}

    public void SetOwnerPlatform(PayloadPlatform platform)
    {
        this.platform = platform;
    }

    public PayloadPlatform GetOwnerPlatform()
    {
        return platform;
    }

    public void ResetToPlatform()
    {
        platform.Reset();
        Destroy(gameObject); // Destroy the duplicate payload, original should still be at the platform
    }
}
