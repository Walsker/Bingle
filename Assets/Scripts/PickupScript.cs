using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Bomb,
    Boost,
    Invisibility,
    None
}

public class PickupScript : MonoBehaviour
{
    public PickupType powerUp;
    public GameObject floatingObject;
    public GameObject[] pickups;
    public GameObject checkerPiece;
    public Color[] checkerColors;
    public float floatDistance;
    public float floatHeight;

    private int randomNumber;
    private float yPosition;
    private PickupType previousType;

    void Start()
    {
        // Generating a random number
        randomNumber = Random.Range(0, 2);

        // Changing the color of the checkerpiece 
        checkerPiece.GetComponent<MeshRenderer>().material.color = checkerColors[randomNumber];

        // Setting the mesh of the item displayed
        floatingObject = (GameObject)Instantiate(pickups[(int)powerUp], transform);
    }

    void Update()
    {
        // Checking if the pickup type has changed
        if (powerUp != previousType)
        {
            // Changing the mesh of the item displayed
            Destroy(floatingObject);
            floatingObject = (GameObject)Instantiate(pickups[(int)powerUp], transform);
            previousType = powerUp;
        }

        // Bobbing the item up and down
        yPosition = floatHeight + Mathf.Sin(Time.time) * floatDistance;

        // Applying the calculated position to the item
        floatingObject.transform.localPosition = new Vector3(0, yPosition);
        floatingObject.transform.rotation = Quaternion.Euler(new Vector3(floatingObject.transform.rotation.eulerAngles.x, floatingObject.transform.rotation.eulerAngles.y + 1f));

        // Checking if there is ground below
        if (!Physics.Raycast(transform.position, Vector3.down, 1))
        {
            // Destroying the pickup
            Destroy(gameObject);
        }

        // Checking if there are too many pickups
        /*if (GameObject.FindGameObjectWithTag("GameController").GetComponent<PickupGenerator>().pickupCount > GameObject.FindGameObjectWithTag("GameController").GetComponent<PickupGenerator>().pickupCap)
        {
            DeletePickup();
        }*/
    }

    void OnTriggerStay(Collider other)
    {
        // Checking if a player hit the pickup
        if (other.gameObject.CompareTag("Player"))
        {
            // Checking if the player already has the ability active
            if (other.gameObject.GetComponent<PickupAbilities>().activeAbility != powerUp && other.gameObject.GetComponent<VehicleController>().allowedToDrive && !other.gameObject.GetComponent<VehicleController>().isUltraCar)
            {
                // Giving the player the ability
                other.gameObject.GetComponent<PickupAbilities>().activeAbility = powerUp;
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Wood") || other.gameObject.CompareTag("Respawn"))
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Destroying the pickup
        GameObject.FindGameObjectWithTag("GameController").GetComponent<PickupGenerator>().pickupCount--;
    }
}
