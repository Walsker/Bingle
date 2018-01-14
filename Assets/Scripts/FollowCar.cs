using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCar : MonoBehaviour {

    public GameObject car;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = car.transform.position + new Vector3(0, 18.9f, -18.9f);
        //transform.position = car.transform.position + new Vector3(0, 100f, -100f);
    }
}
