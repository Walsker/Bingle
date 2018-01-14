using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour {

    public GameObject[] players;

    Rigidbody parent;

    List<Rigidbody> children = new List<Rigidbody>();

	// Use this for initialization
	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (Transform child in transform)
        {
            children.Add(child.GetComponent<Rigidbody>());
            child.tag = "Wood";
        }
    }
	
	// Update is called once per frame
	void Update () {

        // Checking for all the players
        foreach (GameObject player in players)
        {
            // Checking if a player is nearby
            if (Mathf.Abs(player.transform.position.x - transform.position.x) < 100 && Mathf.Abs(player.transform.position.z - transform.position.z) < 100)
            {
                // Making all the blocks not kinematic
                foreach (Rigidbody childRb in children)
                {
                    childRb.isKinematic = false;
                }
            
                // Destroying this script component
                Destroy(this);
            }
        }

        /*if (Mathf.Abs(player2.transform.position.x - transform.position.x) < 100 && Mathf.Abs(player2.transform.position.z - transform.position.z) < 100)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }*/


    }
}
