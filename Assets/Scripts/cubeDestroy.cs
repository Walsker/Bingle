using UnityEngine;
using System.Collections;

public class cubeDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	    
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    void OnCollisionStay(Collision other)
    {
		
        if (other.gameObject.tag == "cubes")
        {
     
			Destroy(gameObject);
		}
   

        //print("test");
    }
}
