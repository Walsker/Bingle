using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {


    //public variables
    public float explosionForce;
    public float ambientForce;
    public float explosionRadius;
    public float upwardsModifier;

    public GameObject explosion;

    //local variables
    public float bombDelay;
    float timeRemaining;

    //the rigidbody of this bomb
    Rigidbody thisRB;

    //this renderer
    Renderer bombRender;

	// Use this for initialization
	void Start ()
    {
        //get this rigidbody
        thisRB = GetComponent<Rigidbody>();

        //get the renderer for this object
        bombRender = GetComponent<Renderer>();

        //get the initial time remaining
        timeRemaining = bombDelay;
	}

    public void ReleaseBomb()
    {
        //delete the script that makes it follow the player
        thisRB.isKinematic = true;
        Destroy(transform.GetComponent<bombFollow>());
        thisRB.isKinematic = false;
        
        //invoke the timer function calling it every 0.1 seconds
        InvokeRepeating("timerTick", 0f, 0.1f);
    }

    void Explode()
    {
        //make sure the bomb doesnt go flying off
        thisRB.isKinematic = true;

        //debugging
        print("exploded");

        //getting all the colliders in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        //for every collider
        foreach (Collider hit in colliders)
        {
            //get the gameObject of the collider
            GameObject body = hit.gameObject;

            //if the object has a parent
            if (body.transform.parent != null)
            {
                //get the parent gameObject
                GameObject parentObject = body.transform.parent.gameObject;

                //if it is a player
                if (parentObject.tag == "Player")
                {
                    //get their rigidbody
                    Rigidbody playerRB = parentObject.GetComponent<Rigidbody>();
                    
                    //if they have one(they should)
                    if (playerRB != null)
                    {
                        //debugging
                        print("player in range");

                        //add explosion force
                        playerRB.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
                    }
                }

            }

            //get the rigidbody component
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            //if rigidbody was found
            if (rb != null)
            {                
                //is the object is not the ground
                if (hit.gameObject.tag != "Ground")
                {        
                    //add explosion force
                    rb.AddExplosionForce(ambientForce, transform.position, explosionRadius, upwardsModifier);                    
                }
            }
        }

        //instantiate the explosion effect
        Instantiate(explosion, transform.position, Quaternion.identity);

        //delete this bomb
        Destroy(gameObject);
    }

    //timer function
    void timerTick()
    {
        //reduce the remaining time
        timeRemaining = timeRemaining- 0.1f;

        //if the bomb is still alive
        if (timeRemaining > 0)
        {
            //switching the material between black and red
            if (bombRender.material.color == Color.red)
            {
                bombRender.material.color = Color.black;
            }
            else
            {
                bombRender.material.color = Color.red;
            }
        }
        //once the time is up call the explode function
        else
        {
            Explode();
        }
    }
}
