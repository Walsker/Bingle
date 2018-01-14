using UnityEngine;
using System.Collections;

public class LobbyCameraMvt : MonoBehaviour
{
    public GameObject target;
    public float turnSpeed;

    void FixedUpdate()
    {
        transform.RotateAround(target.transform.position, Vector3.up, turnSpeed / 50);
        transform.LookAt(target.transform);
    }
}
