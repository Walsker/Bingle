using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    public GameObject target;

    void FixedUpdate()
    {
        Follow();
    }

    // Creating a function that allows this script's parent to mimc the target's movements
    void Follow()
    {
        transform.position = target.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(270, target.transform.rotation.eulerAngles.y, 0));
    }
}
