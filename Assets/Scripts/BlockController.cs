using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {

    AudioSource audioSource;
    AudioClip[] clips;
    AudioClip clipToPlay;
    GameObject player1;

    // Use this for initialization
    void Start () {
        player1 = GameObject.FindGameObjectWithTag("Player");

        clips = new AudioClip[25];
        audioSource = gameObject.AddComponent<AudioSource>();

        //putting all the wood sounds into the list
        clips[0] = (Resources.Load<AudioClip>("wood25") );
        clips[1] = (Resources.Load<AudioClip>("wood1") );
        clips[2] = (Resources.Load<AudioClip>("wood2") );
        clips[3] = (Resources.Load<AudioClip>("wood3") );
        clips[4] = (Resources.Load<AudioClip>("wood4") );
        clips[5] = (Resources.Load<AudioClip>("wood5") );
        clips[6] = (Resources.Load<AudioClip>("wood6") );
        clips[7] = (Resources.Load<AudioClip>("wood7") );
        clips[8] = (Resources.Load<AudioClip>("wood8") );
        clips[9] = (Resources.Load<AudioClip>("wood9") );
        clips[10] = (Resources.Load<AudioClip>("wood10"));
        clips[11] = (Resources.Load<AudioClip>("wood11"));
        clips[12] = (Resources.Load<AudioClip>("wood12"));
        clips[13] = (Resources.Load<AudioClip>("wood13"));
        clips[14] = (Resources.Load<AudioClip>("wood14"));
        clips[15] = (Resources.Load<AudioClip>("wood15"));
        clips[16] = (Resources.Load<AudioClip>("wood16"));
        clips[17] = (Resources.Load<AudioClip>("wood17"));
        clips[18] = (Resources.Load<AudioClip>("wood18"));
        clips[19] = (Resources.Load<AudioClip>("wood19"));
        clips[20] = (Resources.Load<AudioClip>("wood20"));
        clips[21] = (Resources.Load<AudioClip>("wood21"));
        clips[22] = (Resources.Load<AudioClip>("wood22"));
        clips[23] = (Resources.Load<AudioClip>("wood23"));
        clips[24] = (Resources.Load<AudioClip>("wood24"));

    }

    void OnCollisionEnter(Collision collision)
    {
        //if the player is near the current block collision then make the sound
        if (Mathf.Abs(player1.transform.position.x - transform.position.x) < 10 && Mathf.Abs(player1.transform.position.z - transform.position.z) < 10)
        {
            //grabbing a random sound from the list
            clipToPlay = clips[Random.Range(0, clips.Length)];
            audioSource.clip = clipToPlay;

            //changing the volume relative to how hard the colission was
            audioSource.volume = collision.relativeVelocity.magnitude;
            audioSource.Play();
        }

        
    }
}
