using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDeadParticles : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        // Checking if the particle system is still emitting particles
        if (!GetComponent<ParticleSystem>().IsAlive())
        {
            // Destroying the whole object
            Destroy(gameObject);
        }
	}
}
