using UnityEngine;
using System.Collections;

public class VehicleController : MonoBehaviour
{
    public GameObject[] wheels = new GameObject[4]; // 0 and 1 are the front wheels, 2 and 3 are the rear wheels
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public bool allowedToDrive = false;
    public float maxSteerAngle;
    [Range(0, 1)] public float tractionControl;
    [Range(0, 1)] public float steerControl;
    public float totalTorqueOverAllWheels;
    public float reverseTorque;
    public float brakeTorque;
    public float flipResist;
    public float topSpeed;
    public float slipLimit;
    public float flipPower;
    public GameObject centreOfMass;
    public GameObject playerBody;
    public Material[] playerBodies;
    public int playerID;
    public ParticleSystem ultraParticlePrefab;
    public bool displayMode;

    [HideInInspector]
    public Color[] playerColors;
    [HideInInspector]
    public bool isUltraCar = false;
    [HideInInspector]
    public Camera playerCamera;
    [HideInInspector]
    public bool bingled;

    private JointMotor wheelMotor;
    private float appliedTorque;
    private float speed;
    private float oldRotation;
    private Rigidbody rb;
    private ParticleSystem ultraParticles;
    private int previousID;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerColors = StaticInfo.PlayerColors;

        // Setting the centre of mass of the car to a specific place so it is not calculated by unity
        rb.centerOfMass = centreOfMass.transform.localPosition;

        // The amount of torque applied when traction control is taken into account
        appliedTorque = totalTorqueOverAllWheels - (totalTorqueOverAllWheels * tractionControl);
    }

    void Start()
    {
        if (!displayMode)
        {
            playerCamera = GameObject.FindWithTag("GameController").GetComponent<SpawnPlayerScript>().playerCameras[playerID - 1];
        }

        // Changing the color of the car
        playerBody.GetComponent<Renderer>().material = playerBodies[playerID - 1];
    }

    void Update()
    {
        if (previousID != playerID)
        {
            // Changing the color of the car
            playerBody.GetComponent<Renderer>().material = playerBodies[playerID - 1];
            previousID = playerID;
        }

    }
    // A function that updates the wheel mesh positions according to the wheelColliders' 
    void UpdateMesh()
    {
        // Iterating through all the wheelcolliders
        for (int c = 0; c < 4; c++)
        {
            Quaternion rotation;
            Vector3 position;

            // Retrieving the position and rotation of the wheel collider
            wheelColliders[c].GetWorldPose(out position, out rotation);

            // Applying the newfound information to the wheel meshes
            wheels[c].transform.position = position;
            wheels[c].transform.rotation = rotation;
        }
    }

    // A function that is called continuously on a very small interval  
    void FixedUpdate ()
    {
        float steeringAngle;
        float motorTorque;

        // Checking if the player is still in the round
        if (!bingled && allowedToDrive)
        {
            // Getting input from the player
            steeringAngle = Input.GetAxis("P" + playerID + "Horizontal");
            motorTorque = Input.GetAxis("P" + playerID + "Vertical");
        }
        else
        {
            steeringAngle = 0;
            motorTorque = 0;
        }

        // Finding the speed of the car
        speed = rb.velocity.magnitude;

        // Moving the car
        Move(steeringAngle, motorTorque);
        UpdateMesh();
    }

    // Creating a pair of functions that enable and disable the ability to control the player
    public void Enable()
    {
        allowedToDrive = true;
    }
    public void Disable()
    {
        allowedToDrive = false;
    }

    // Creating a function that moves the card according to the player's input
    void Move(float steering, float torque)
    {
        // Rotating the wheels according to the player's input
        wheelColliders[0].steerAngle = steering * maxSteerAngle; wheels[0].transform.localRotation = Quaternion.Euler(new Vector3(0, steering * maxSteerAngle));
        wheelColliders[1].steerAngle = steering * maxSteerAngle; wheels[1].transform.localRotation = Quaternion.Euler(new Vector3(0, steering * maxSteerAngle));

        // Applying forward and backward movement
        ApplyTorques(Mathf.Clamp(torque, 0, 1), Mathf.Clamp(torque, -1, 0));

        // Checking if the car needs to flip over
        if (Vector3.Angle(Vector3.down, transform.TransformDirection(Vector3.down)) > 50f)
        {
            FlipRecovery(-steering);
        }

        // Resisting the tendency to flip at high velocities
        FlipPrevention();

        // Limiting the speed of the car for safety reasons
        LimitSpeed();

        // Simulating extra traction (dependant on tractionControl)
        TractionControl();

        // Helping the user steer the car
        SteerHelper();
    }

    void ApplyTorques(float acceleration, float brakeForce)
    {
        // Applying torque to all the wheels
        for (int i = 0; i < 4; i++)
        {
            //print("Acc.: " + acceleration + " | BF: " + brakeForce + " | 2nd: " + (speed > 5) + " and " + (Vector3.Angle(transform.forward, rb.velocity) < 45f) + " | " + Time.time);
            // Checking if there is forward input
            if (acceleration > 0)
            {
                //print(acceleration + "  1");
                wheelColliders[i].brakeTorque = 0;
                wheelColliders[i].motorTorque = acceleration * (appliedTorque / 4f);
            }
            // Checking if the vehicle is coming to a halt from a fast velocity
            else if (speed > 5 && Vector3.Angle(transform.forward, rb.velocity) < 45f && brakeForce < 0)
            {
                //print((speed > 5) + "  2");
                wheelColliders[i].brakeTorque = brakeTorque * -brakeForce;
            }
            // Checking if there is backwards input
            else if (brakeForce < 0)
            {
                //print(brakeForce + "  3");
                wheelColliders[i].brakeTorque = 0f;
                wheelColliders[i].motorTorque = reverseTorque * brakeForce;
            }
            else
            {
                //print(speed + "  4");
                wheelColliders[i].brakeTorque = 20000f;
            }
        }
    }

    // Creating a function that allows the player to flip over
    void FlipRecovery(float force)
    {
        rb.AddRelativeTorque(Vector3.forward * force * flipPower);
    }

    // Creating a function that stops the car from flipping at high velocities
    void FlipPrevention()
    {
        rb.AddForce(-transform.up * flipResist * rb.velocity.magnitude);
    }

    // Creating a function that helps the car make sharper
    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            wheelColliders[i].GetGroundHit(out wheelhit);

            // Checking if the wheels are not on the ground
            if (wheelhit.normal == Vector3.zero)
            {
                return;
            }
        }

        // The if statement prevents gimbal locks from directly modifying the rotation of the already fast moving car
        if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
        {
            float turnAdjustment = (transform.eulerAngles.y - oldRotation) * steerControl;
            Quaternion rotationFix = Quaternion.AngleAxis(turnAdjustment, Vector3.up);
            rb.velocity = rotationFix * rb.velocity;
        }
        oldRotation = transform.eulerAngles.y;
    }

    // Creating a function that limits the speed of the car
    void LimitSpeed()
    {
        // Checking if the car has reached the speed limit
        if (speed > topSpeed)
        {
            // Setting the car's velocity to the speed limit
            rb.velocity -= topSpeed / 50 * rb.velocity.normalized;
        }
    }

    void TractionControl()
    {
        WheelHit wheelHit;

        // Looping through the wheels
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetGroundHit(out wheelHit);

            AdjustTorque(wheelHit.forwardSlip);
        }
    }
     
    void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= slipLimit && appliedTorque >= 0)
        {
            appliedTorque -= 10 * tractionControl;
        }
        else
        {
            appliedTorque += 10 * tractionControl;
        }
    }

    // Creating a function that enlarges the player
    public void UltraMagnify(float magnitude)
    {
        // Creating a particle effect for the player
        ultraParticles = (ParticleSystem)Instantiate(ultraParticlePrefab);
        ultraParticles.GetComponent<Mimic>().target = gameObject;
        ultraParticles.transform.localScale *= 3;
        ParticleSystem.MainModule mainPointer = ultraParticles.GetComponent<ParticleSystem>().main;
        mainPointer.startColor = playerColors[playerID - 1]; // Don't use main. because it doesn't really work even though it's encouraged

        // Scaling the player up
        transform.localScale *= magnitude;
        totalTorqueOverAllWheels *= magnitude;
        topSpeed *= 0.5f;
        flipPower *= magnitude;
        maxSteerAngle *= 2f/magnitude;
        GameObject.FindWithTag("GameController").GetComponent<SpawnPlayerScript>().playerCameras[playerID - 1].GetComponent<CameraFollow>().maxCameraDistance *= magnitude / 2;
    }

    public void UltraNormify(float magnitude)
    {
        // Removing the particles
        Destroy(ultraParticles);

        // Scaling the player back down
        transform.localScale /= magnitude;
        totalTorqueOverAllWheels /= magnitude;
        topSpeed /= 0.75f;
        flipPower /= magnitude;
        maxSteerAngle /= 2f/magnitude;
        GameObject.FindWithTag("GameController").GetComponent<SpawnPlayerScript>().playerCameras[playerID - 1].GetComponent<CameraFollow>().maxCameraDistance /= magnitude / 2;
    }

    // Creating a function that applies forces to things
    void OnTriggerEnter(Collider other)
    {
        if (isUltraCar || GetComponent<PickupAbilities>().boostEnabled)
        {
            UltraPowers(other);
        }
    }
        
    // Creating a function that pushes away blocks from the ultracar
    void UltraPowers(Collider unfortunateObject)
    {
        // Checking if the block is wood
        if ((unfortunateObject.gameObject.CompareTag("Wood") && isUltraCar) || unfortunateObject.gameObject.CompareTag("Player"))
        {
            // Reducing the force if it's a boosting normal player
            float extraForce = 75f * unfortunateObject.gameObject.GetComponent<Rigidbody>().mass;
            if (!isUltraCar)
            {
                extraForce /= 2;
            }

            // Adding force to it
            unfortunateObject.attachedRigidbody.AddForce((unfortunateObject.transform.position - transform.position + Vector3.up / 2) * Mathf.Clamp((rb.velocity.magnitude + 10f) * extraForce, 0f, 1000000f));
            print(Mathf.Clamp((rb.velocity.magnitude + 10f) * extraForce, 0f, 100000f));
            // Checking if it's another player
            if (unfortunateObject.gameObject.CompareTag("Player") && isUltraCar)
            {
                // Checking if the player is not a carcass
                if (!unfortunateObject.gameObject.GetComponent<VehicleController>().bingled)
                {
                    unfortunateObject.gameObject.GetComponent<VehicleController>().Bingle();
                    StaticInfo.PlayerPoints[playerID - 1]++;
                }
            }

        }
    }

    // Creating a function for if the player is out of the game
    public void Bingle()
    {
        // Stopping the player for inputting movement
        bingled = true;

        // Telling the game controller
        GameObject.FindWithTag("GameController").GetComponent<CommonGCMethods>().bingleCount++;

        // Removing pickups
        GetComponent<PickupAbilities>().activeAbility = PickupType.None;

        // Making the camera fade out
        playerCamera.GetComponent<CameraFollow>().GoGray();
    }
}
