using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour {

    Rigidbody rb;

    IEnumerator fallCoroutine;

    RaycastHit hitInfo;

    [HideInInspector]
    public bool active;

    private IEnumerator StartFall(float waitTime, Rigidbody toDestroy)
    {
        yield return new WaitForSeconds(waitTime);

        //to make the platform fall simply make is not kinematic, and give it a small downward force
        toDestroy.isKinematic = false;
        toDestroy.AddForce(0, -2, 0);
        toDestroy.transform.localScale = toDestroy.transform.localScale * .95f;
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, 2f) && Time.timeSinceLevelLoad > 5 && active)
        {
            if (hitInfo.transform.gameObject.tag == "Platform")
            {
                rb = hitInfo.transform.gameObject.GetComponent<Rigidbody>();
                StartCoroutine(StartFall(1f, rb));
            }
        }
    }
}
