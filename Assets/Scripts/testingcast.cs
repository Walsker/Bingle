using UnityEngine;
using System.Collections;

public class testingcast : MonoBehaviour {

    public GameObject otherObject;

    Vector3 heading;

    float distance;

    Vector3 direction;

    RaycastHit[] hits;

    // Use this for initialization
    void Start () {
        heading = otherObject.transform.position - gameObject.transform.position;

        distance = heading.magnitude;
        direction = heading / distance;

        hits = Physics.RaycastAll(transform.position, direction, distance);

        foreach(RaycastHit toDestroy in hits)
        {
            if (toDestroy.transform.gameObject.tag != "Ground")
            {
                GameObject.Destroy(toDestroy.transform.gameObject);
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
