using UnityEngine;
using System.Collections;

public class Sphere : MonoBehaviour {

    int numberOfCollisions;
    Rigidbody rb;
    float currentScale;
    int numberOfLines = 0;

    public float thickness = 1;

    //raycasting
    Vector3 heading;
    float distance;
    Vector3 direction;
    RaycastHit[] hits;

    //for deletion
    public bool doneGrowing = false;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        if (numberOfCollisions < 2)
        {
            currentScale = currentScale + 4f;

            rb.gameObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }
        else
        {
            doneGrowing = true;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "cube")
        {
            if (numberOfCollisions < 2)
            {
                numberOfLines = numberOfLines + 2;

                heading = other.transform.position - gameObject.transform.position;
                distance = heading.magnitude;
                direction = heading / distance;

                hits = Physics.SphereCastAll(transform.position, thickness, direction, distance);

                foreach (RaycastHit toDestroy in hits)
                {
                    if (toDestroy.transform.gameObject.tag == "Wood")
                    {
                        GameObject child = toDestroy.transform.gameObject;
                        //print(child.transform.parent.name);
                        try
                        {
                            Destroy(child.transform.parent.gameObject);
                        }
                        catch
                        {

                            Debug.LogError("Could not delete '" + child.transform.parent.name + "' with sphere!");
                        }
                    }
                }
            }
            numberOfCollisions++;
        }
    }
}
