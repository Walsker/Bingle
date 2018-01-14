using UnityEngine;
using System.Collections;

public enum ScreenTypes
{
	Single,
	Double,
	Quad
}

public class CameraFollow : MonoBehaviour
{
    public int cameraID;
    public float cameraAngle;
    public float cameraHeight;
    public float maxCameraDistance;
    public float turnSpeed;

    [HideInInspector]
    public ScreenTypes type;
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public bool unassigned;

    // Variables for following the player
	private Vector3 cameraPosition;
	private GameObject cameraTarget;
    private Vector3 cameraOffset;
    private float cameraAngleRAD;
    private bool bingled;

    private UnityStandardAssets.ImageEffects.Grayscale grayEffect;
    private UnityStandardAssets.ImageEffects.NoiseAndGrain grainEffect;

    // Variables for the camera dipping functions
    private float defaultAngle;
    private float defaultFOV;
    private float defaultColorBalance;
    private bool currentlyDipping;
    private bool stalling;
    private float angleDecrease;
    private float dipDuration;
    private float stallTime;
    private float dipStartTime;
    private float dipReturnTime;
    private float dipProgress;
    private float startAngle;
    private float endAngle;
    private float startFOV;
    private float endFOV;
    private float startColor;
    private float endColor;

    void Awake()
    {
        // Saving the default angle and fov
        defaultAngle = cameraAngle;
        defaultFOV = GetComponent<Camera>().fieldOfView;
        defaultColorBalance = 0;
    }

	// Use this for initialization
	void Start ()
	{
        // Retrieving the black and white effects and disabling them
        grayEffect = GetComponent<UnityStandardAssets.ImageEffects.Grayscale>();
        grayEffect.enabled = false;
        grainEffect = GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain>();
        grainEffect.enabled = false;

		// Creating a target
		cameraTarget = new GameObject();

		// Setting the camera's splitscreen postiion
        if (type == ScreenTypes.Single)
		{
            // Setting the camera to take up the whole screen
			GetComponent<Camera>().rect = new Rect(new Vector2(0f, 0f), new Vector2(1f, 1f));
		}
        else if (type == ScreenTypes.Double)
		{
            if (cameraID == 1)
            {   
                // Setting the camera to take up the top half of the screen
                GetComponent<Camera>().rect = new Rect(new Vector2(0f, 0.5f), new Vector2(1f, 0.5f));
            }
            else if (cameraID == 2)
            {
                // Setting the camera to take up the bottom half of the screen
                GetComponent<Camera>().rect = new Rect(new Vector2(0f, 0f), new Vector2(1f, 0.5f));
            }
		}
		else if (type == ScreenTypes.Quad)
		{
            if (cameraID == 1)
            {   
                // Setting the camera to take up the top-left quadrant of the screen
                GetComponent<Camera>().rect = new Rect(new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f));
            }
            else if (cameraID == 2)
            {
                // Setting the camera to take up the top-right quadrant of the screen
                GetComponent<Camera>().rect = new Rect(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            }
            else if (cameraID == 3)
            {
                // Setting the camera to take up the bottom-left quadrant of the screen
                GetComponent<Camera>().rect = new Rect(new Vector2(0f, 0f), new Vector2(0.5f, 0.5f));
            }
            else if (cameraID == 4)
            {
                // Setting the camera to take up the bottom-right quadrant of the screen
                GetComponent<Camera>().rect = new Rect(new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f));
            }
		}

		// Setting the camera's target to its initial position
		UpdateTargetPosition();
	}

	void FixedUpdate()
	{
        // Turnging the camera angle into a vector
        cameraAngleRAD = cameraAngle * Mathf.Deg2Rad;
        cameraOffset = new Vector3(0, maxCameraDistance * Mathf.Sin(cameraAngleRAD), -maxCameraDistance * Mathf.Cos(cameraAngleRAD));

		UpdateTargetPosition();
	}

    // Creating a function that moves the camera's target
	void UpdateTargetPosition()
	{
        // Moving the target's positions
        cameraTarget.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 2.05f + cameraHeight, target.transform.position.z);

        // Checking if the camera should spin
        if (bingled || unassigned)
        {
            cameraTarget.transform.rotation = Quaternion.Euler(new Vector3(0, cameraTarget.transform.rotation.eulerAngles.y + turnSpeed, 0));
        }
        else
        {
            cameraTarget.transform.rotation = Quaternion.Euler(new Vector3(0, target.transform.rotation.eulerAngles.y, 0));
        }
	}

	void LateUpdate()
	{
        // Moving the camera
        MoveCamera();

        // Checking if there is a request to dip the camera
        if (currentlyDipping)
        {
            DipCamera();
        }
	}

	void MoveCamera()
	{
		// Finding the target's position and rotation and positioning the camera properly
        RaycastHit rayInfo;

        // Checking if there are any objects in the way of the camera's vision
        if (Physics.Raycast(cameraTarget.transform.position, cameraTarget.transform.TransformDirection(cameraOffset.normalized), out rayInfo, maxCameraDistance))
        {
            if (!rayInfo.transform.gameObject.CompareTag("Player") && !rayInfo.collider.isTrigger)
            {
                // Setting the camera's positions to wherever the raycast hit
                cameraPosition = rayInfo.point;
                //print(rayInfo.transform.ToString() + " | " + Time.time);
            }
            else
            {
                // Setting the camera's position to the farthest point in the ray
                cameraPosition = cameraTarget.transform.position + cameraTarget.transform.TransformDirection(cameraOffset.normalized * maxCameraDistance);
            }
        }
        else
        {
            // Setting the camera's position to the farthest point in the ray
            cameraPosition = cameraTarget.transform.position + cameraTarget.transform.TransformDirection(cameraOffset.normalized * maxCameraDistance);
        }

        transform.position = cameraPosition;
        transform.LookAt(cameraTarget.transform);
	}

    // Creating functions that drop the camera angle by a certain angle and returns it after a duration
    public void StartDipping(float angle, float duration, float stall, float stretch, float color)
    {
        // Checking if the camera is already in a dipping loop
        if (!currentlyDipping)
        {
            // Handing off the arguments so the camera knows what to do
            angleDecrease = angle;
            dipDuration = duration;
            stallTime = stall;

            dipStartTime = Time.time;
            dipReturnTime = Time.time + dipDuration + stallTime;

            startAngle = cameraAngle;
            dipProgress = 0f;
            startFOV = defaultFOV;
            endFOV = defaultFOV + stretch;
            startColor = defaultColorBalance;
            endColor = defaultColorBalance + color;

            // Checking if the camera is coming from the front or the back
            if (startAngle <= 90f)
            {
                endAngle = startAngle - angleDecrease;
            }
            else
            {
                endAngle = startAngle + angleDecrease;
            }

            // Enabling the looping dip function in Update()
            currentlyDipping = true;
            stalling = false;
        }

    }
    void DipCamera()
    {
        // Checking if the camera is stalling in a low position
        if (stalling)
        {
            // Checking if it's time for the camera to return to it's normal position
            if (Time.time > dipReturnTime)
            {
                stalling = false;

                // Changing the end values to the default to return to normal
                endAngle = defaultAngle;
                startAngle = cameraAngle;
                endFOV = defaultFOV;
                startFOV = GetComponent<Camera>().fieldOfView;
                endColor = defaultColorBalance;
                startColor = GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>().chromaticAberration;

                dipStartTime = Time.time;
            }
        }

        // Finding out how close the camera is to the destination (0 - at the start, 1 - at the end)
        dipProgress = (Time.time - dipStartTime) / dipDuration;

        // Lerping all the intensity values
        cameraAngle = Mathf.LerpAngle(startAngle, endAngle, dipProgress);
        GetComponent<Camera>().fieldOfView = Mathf.Lerp(startFOV, endFOV, dipProgress);
        GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>().chromaticAberration = Mathf.Lerp(startColor, endColor, dipProgress);

        // Checking if the first half of the dip is complete
        if (dipProgress > 1.0f)
        {
            // Checking if the camear has properly returned to it's original spot
            if (cameraAngle == defaultAngle)
            {
                // Ending the dipping loop
                currentlyDipping = false;
            }
            else
            {
                stalling = true;
                dipProgress = 0f;
            }
        }
    }

    // Creating a public function that starts the game over sequence
    public void GoGray()
    {
        StartCoroutine(GameOverSequence());
    }
    IEnumerator GameOverSequence()
    {
        // Enabling the grayscale component and moving the camera close
        grayEffect.enabled = true;
        grainEffect.enabled = true;
        bingled = true;

        yield return new WaitForSeconds(1.25f);

        GameOver();
    }

    // Creating a function that displays "Bingle" on the player's camera
    void GameOver()
    {
        //TODO: Display Bingle! properly
    }
}
