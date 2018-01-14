using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombFollow : MonoBehaviour {

    public GameObject owner;

	// Update is called once per frame
	void Update ()
    {
        //follow the car
        transform.position = owner.transform.TransformPoint(0, 2, -5);
	}
}
