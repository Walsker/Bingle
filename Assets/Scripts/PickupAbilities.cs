using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PickupAbilities : MonoBehaviour
{
    #region General Information
    public PickupType activeAbility;

    private VehicleController mainScript;
    private string abilityKey;
    private Rigidbody rb;
    private int playerID;
    #endregion

    #region Bomb Information
    public GameObject bombPrefab;

    private bool isBombActive = false;
    private GameObject bombInstance;
    #endregion

    #region Boost Information
    public float boostMagnitude;
    public float boostDuration;
    public float cameraAngleDecrease;
    public float FOVStretch;
    public float colorShift;
    public ParticleSystem fireFXPrefab;
    public GameObject boostPrefab;

    [HideInInspector]
    public bool boostEnabled;

    private Camera playerCamera;
    private float timeToEndBoost;
    private bool boostReady;
    private ParticleSystem[] fireFX;
    private GameObject boostItemInstance;
    private bool flamingTires;
    #endregion

    #region Invisibility Information
    public Material invisibleMaterial;
    public ParticleSystem lightExplosion;
    public ParticleSystem lightImplosion;
    public float invisibilityDuration;

    private bool currentlyInvisible;
    private float revealTime;
    private int defaultCullingMask;
    private List<List<Material>> playerMaterials;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainScript = GetComponent<VehicleController>();
        fireFX = new ParticleSystem[2];
    }
    void Start()
    {
        playerID = mainScript.playerID;
        abilityKey = "P" + playerID + "Ability";
        playerCamera = GameObject.FindWithTag("GameController").GetComponent<SpawnPlayerScript>().playerCameras[playerID - 1];

        #region Bomb Section
        #endregion

        #region Boost Section
        // Creating tracks of fire for the car
        fireFX[0] = (ParticleSystem)Instantiate(fireFXPrefab); 
        fireFX[0].gameObject.AddComponent<Mimic>().target = mainScript.wheels[2];

        fireFX[1] = (ParticleSystem)Instantiate(fireFXPrefab);
        fireFX[1].gameObject.AddComponent<Mimic>().target = mainScript.wheels[3];
        #endregion

        #region Invisibility Section
        #endregion
    }

    void FixedUpdate()
    {
        #region Bomb Section
        // Checking if the bomb pickup has been collected
        if (activeAbility == PickupType.Bomb && !isBombActive)
        {
            // Instantiate the actual bomb
            bombInstance = Instantiate(bombPrefab, transform.position, Quaternion.Euler(-60, 30, -25)) as GameObject;

            // Let the bomb know which object it has to follow
            bombInstance.GetComponent<bombFollow>().owner = gameObject;
            isBombActive = true;
        }

        // Aligning the bomb to the car
        if (isBombActive)
        {
            bombInstance.transform.rotation = Quaternion.Euler(new Vector3(playerCamera.GetComponent<CameraFollow>().target.transform.rotation.eulerAngles.x - 45f, playerCamera.GetComponent<CameraFollow>().target.transform.rotation.eulerAngles.y + 90f));
        }

        // Checking if the player presses their ability button
        if (isBombActive && Input.GetButtonDown(abilityKey))
        {
            // Releasing the bomb and turning off the bomb ability
            bombInstance.GetComponent<Bomb>().ReleaseBomb();
            activeAbility = PickupType.None;
            isBombActive = false;
        }

        // Checking if another pickup was collected while the bomb was still active
        if (isBombActive && !(activeAbility == PickupType.Bomb))
        {
            // Destroying the bomb
            Destroy(bombInstance);
            isBombActive = false;
        
        }
        #endregion

        #region Boost Section
        // Checking if the boost pickup has been collected
        if (activeAbility == PickupType.Boost)
        {
            // Checking if the boost item hasn't been created yet
            if (!boostReady)
            {
                // Creating a boost item above the player
                boostItemInstance = (GameObject)Instantiate(boostPrefab);
                boostReady = true;
            }

            if (boostReady)
            {
                // Following the player
                boostItemInstance.transform.position = transform.TransformPoint(new Vector3(0, 3f, -2f));
                boostItemInstance.transform.rotation = transform.rotation;
            }
        }

        // Checking if the boost ability has been acitvated
        if (Input.GetButtonDown(abilityKey) && !boostEnabled && activeAbility == PickupType.Boost)
        {
            // Enabling the boost function to loop, defining its end and increasing the car's speed limit
            boostEnabled = true;
            mainScript.topSpeed *= 2;
            timeToEndBoost = Time.time + boostDuration;

            // Enabling the tracks of fire
            fireFX[0].Play();
            fireFX[1].Play();
            flamingTires = true;

            // Removing the floating item above the player
            Destroy(boostItemInstance);

            // Removing the boost ability from the player
            activeAbility = PickupType.None;
            boostReady = false;
        }

        // Checking if the boost in on
        if (boostEnabled)
        {
            // Increasing speed
            Vector3 additionalVelocity = transform.TransformDirection(Vector3.forward) * boostMagnitude / 10;
            rb.velocity += new Vector3(additionalVelocity.x, 0f, additionalVelocity.z);

            // Warping camera
            playerCamera.GetComponent<CameraFollow>().StartDipping(cameraAngleDecrease, boostDuration * 2/3, boostDuration * 1/3, FOVStretch, colorShift);

            // Checking if the player is on the ground
            if (!flamingTires && (transform.position.y < GameObject.FindWithTag("GameController").GetComponent<cityGenerator>().CityHeight - 0.5f || transform.position.y > GameObject.FindWithTag("GameController").GetComponent<cityGenerator>().CityHeight - 1.5f))
            {
                // Enabling the tracks of fire
                fireFX[0].Play();
                fireFX[1].Play();
                flamingTires = true;
            }
            else if (flamingTires && (transform.position.y > GameObject.FindWithTag("GameController").GetComponent<cityGenerator>().CityHeight - 0.5f || transform.position.y < GameObject.FindWithTag("GameController").GetComponent<cityGenerator>().CityHeight - 1.5f))
            {
                // Disabling the tracks of fire
                fireFX[0].Stop();
                fireFX[1].Stop();
                flamingTires = false;
            }

            // Checking if the boost is over
            if (Time.time > timeToEndBoost)
            {
                boostEnabled = false;

                // Reducing the car's speed limit
                mainScript.topSpeed /= 2;

                // Disabling the tracks of fire
                fireFX[0].Stop();
                fireFX[1].Stop();
                flamingTires = false;
            }
        }
        else
        {
            // Hiding the trails of fire
            fireFX[0].Stop();
            fireFX[1].Stop();
            flamingTires = false;
        }

        // Checking if another pickup was collected while the boost was still ready
        if (boostReady && !(activeAbility == PickupType.Boost))
        {
            // Destroying the boost item above the player
            Destroy(boostItemInstance);
            boostReady = false;

        }
        #endregion

        #region Invisibility Section
        // Checking if the invisibilty pickup has been collected
        if (activeAbility == PickupType.Invisibility && !currentlyInvisible)
        {
            // Creating an explosion of paricles at the player's location
            Instantiate(lightExplosion, transform.position, Quaternion.identity);

            // Making the player transparent and moving them to a different layer (invisible to other cameras)
            Renderer[] allRends = GetComponentsInChildren<Renderer>();
            Transform[] allTransforms = GetComponentsInChildren<Transform>();
            playerMaterials = new List<List<Material>>();

            // Finding all the renderers and transforms in the children
            foreach (Renderer rend in allRends)
            {   
                List<Material> glass = new List<Material>();

                // Going through the array of materials
                for (int r = 0; r < rend.materials.Length; r++)
                {
                    glass.Add(invisibleMaterial);
                }

                // Saving each of the materials before making them glass
                playerMaterials.Add(rend.materials.ToList());
                rend.materials = glass.ToArray();
            }
            foreach(Transform t in allTransforms)
            {
                t.gameObject.layer = 11;
            }

            // Letting the player's camera see the player on that layer
            defaultCullingMask = playerCamera.cullingMask;
            playerCamera.cullingMask = -1;

            revealTime = Time.time + invisibilityDuration;
            currentlyInvisible = true;
        }

        // Checking if the time is up or another pickup was collected while invisible
        if (currentlyInvisible && (Time.time > revealTime || !(activeAbility == PickupType.Invisibility)))
        {
            // Creating an implosion of paricles in front of the player's location
            Instantiate(lightImplosion, transform.position + GetComponent<Rigidbody>().velocity / 5f, Quaternion.identity);

            // Changing the player's material back to normal
            Renderer[] allRends = GetComponentsInChildren<Renderer>();
            Transform[] allTransforms = GetComponentsInChildren<Transform>();

            // Moving the player to a back to the normal layer
            foreach (Renderer rend in allRends)
            {
                // Retrieving each of the materials
                rend.materials = playerMaterials[0].ToArray();
                playerMaterials.RemoveAt(0);

            }
            foreach(Transform t in allTransforms)
            {
                t.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            // Disabling the camera's ability to see on that layer
            playerCamera.cullingMask = defaultCullingMask;
            currentlyInvisible = false;
            if (activeAbility == PickupType.Invisibility)
            {
                activeAbility = PickupType.None;
            }
        }
        #endregion
    }
}