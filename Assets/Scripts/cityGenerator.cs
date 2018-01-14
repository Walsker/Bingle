using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class cityGenerator : MonoBehaviour {

    public GameObject sphere;
    public float blockMass = 1000;

    public List<GameObject> buildings = new List<GameObject>();
    public List<GameObject> trees = new List<GameObject>();

    List<Vector3> listOfPoints = new List<Vector3>();

    List<GameObject> spheres = new List<GameObject>();

    public float minDistance = 0.25f;

    Vector3 newLocation;

    [Range(3, 200)] public int intersections;

    bool worksOut = true;

    public int numSecondsToWait;

    public float roadWidth;

    public bool loaded = false;

	public int centreMaxDistance;
	public int centreMinDistance;

    public float CityHeight = 1;

    public float CityHalfSize = 500;

    public float BuildingTries = 2500;

    //raycasting
    Vector3 heading;
    float distance;
    Vector3 direction;
    RaycastHit[] hits;

    void Awake ()
    {
        Time.timeScale = 1;

        // Telling the game that level is not ready
        StaticInfo.LevelIsReady = false;
    }

    // Use this for initialization
    void Start ()
    {

        GenerateCubes();

        CreateIntersections();

    }

    private void GenerateCubes()
    {
        GameObject currentBuilding;
        GameObject placedBuilding;
        BoxCollider buildingCollider;
        Vector3 buildingLocation;
        Quaternion buildingRotation;
        Rigidbody childRb;

        GameObject currentTree;
        GameObject placedTree;
        BoxCollider treeCollider;
        Vector3 treeLocation;
        Quaternion treeRotation;

        for (int k = 0; k < BuildingTries; k++)
        {
            currentBuilding = buildings[Random.Range(0, buildings.Count)];

            buildingCollider = currentBuilding.GetComponent<BoxCollider>();

            buildingLocation = new Vector3(Random.Range(-CityHalfSize, CityHalfSize), CityHeight, Random.Range(-CityHalfSize, CityHalfSize));
            buildingRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);

            

            if (Physics.OverlapBox(buildingLocation + new Vector3(0, buildingCollider.size.y * currentBuilding.transform.lossyScale.y / 2, 0), buildingCollider.size * currentBuilding.transform.lossyScale.y / 2, buildingRotation).Length < 0.01)
            {
                placedBuilding = Instantiate(currentBuilding, buildingLocation, buildingRotation) as GameObject;
                

                foreach (Transform child in placedBuilding.transform)
                {
                    childRb = child.GetComponent<Rigidbody>();
                    childRb.isKinematic = true;
                    child.gameObject.AddComponent<BlockController>();

                    if (placedBuilding.tag != "light")
                    {
                        childRb.mass = blockMass;


                        float randomColour = Random.Range(0, 12);

                        if (randomColour == 0)
                        {
                            child.GetComponent<MeshRenderer>().material.color = new Color(1, .2f, .2f);
                        }
                        else if (randomColour == 1)
                        {
                            child.GetComponent<MeshRenderer>().material.color = new Color(.2f, .8f, .2f);
                        }
                        else if (randomColour == 2)
                        {
                            child.GetComponent<MeshRenderer>().material.color = new Color(.35f, .4f, 1);
                        }

                    }
                    else
                    {
                        childRb.mass = 1;
                    }

                }

                placedBuilding.gameObject.AddComponent<Magic>();
                Destroy(placedBuilding.GetComponent<BoxCollider>());
            }

        }
        for (int l = 0; l < BuildingTries/2; l++)
        {
            currentTree = trees[Random.Range(0, trees.Count)];

            treeCollider = currentTree.GetComponent<BoxCollider>();

            treeLocation = new Vector3(Random.Range(-CityHalfSize, CityHalfSize), CityHeight, Random.Range(-CityHalfSize, CityHalfSize));
            treeRotation = Quaternion.Euler(0f, Random.Range(0, 260), 0f);

            if (Physics.OverlapBox(treeLocation + new Vector3(0, treeCollider.size.y * currentTree.transform.lossyScale.y / 2, 0), treeCollider.size * currentTree.transform.lossyScale.y / 2, treeRotation).Length < 0.01)
            {
                placedTree = Instantiate(currentTree, treeLocation, treeRotation) as GameObject;
                

                foreach (Transform child in placedTree.transform)
                {
                    childRb = child.GetComponent<Rigidbody>();
                    childRb.isKinematic = true;
                    childRb.mass = 1;
                    child.gameObject.AddComponent<BlockController>();
                }

                placedTree.gameObject.AddComponent<Magic>();

                Destroy(placedTree.GetComponent<BoxCollider>());
            }
        }
    }
        

    private void CreateIntersections()
    {
        for (int i = 0; i < intersections; i++)
        {
            newLocation = new Vector3(Random.Range(-CityHalfSize, CityHalfSize), CityHeight, Random.Range(-CityHalfSize, CityHalfSize));


            foreach (Vector3 point in listOfPoints)
            {
                if ((Mathf.Abs(newLocation.x - point.x) > minDistance) && (Mathf.Abs(newLocation.z - point.z) > minDistance))
                {
                    worksOut = true;
                }
                else
                {
                    worksOut = false;
                    break;
                }
            }

            if (worksOut == true)
            {
                listOfPoints.Add(newLocation);
                //---------------------------------------------------------------------------------------------------------------------------------------------------
                GameObject currentSphere = Instantiate(sphere, newLocation, Quaternion.identity) as GameObject;
                spheres.Add(currentSphere);

                worksOut = false;
            }
        }
    }

	// Update is called once per frame
	void Update () 
    {
        if (loaded == false)
        {
            bool canDelete = true;

            foreach (GameObject sphereToCheck in spheres)
            {
                if (sphereToCheck.GetComponent<Sphere>().doneGrowing == false)
                {
                    canDelete = false;
                }
            }


            if (canDelete == true)
            {
                foreach (GameObject sphereToDelete in spheres)
                {
                    Destroy(sphereToDelete);
                }
                loaded = true;

                // Telling the game that the level is ready
                StaticInfo.LevelIsReady = true;
            }
        }

        if (Input.GetKeyDown("space"))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}
