using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightDeath : MonoBehaviour
{

	// Update is called once per frame
	void Update ()
    {
        //if the player is definitely falling off the platform
        if (transform.position.y < 80f && !GetComponent<VehicleController>().bingled)
        {
            GetComponent<VehicleController>().Bingle();
        }	
	}
}
