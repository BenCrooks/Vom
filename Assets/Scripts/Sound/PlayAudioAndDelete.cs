using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAndDelete : MonoBehaviour
{
    public AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GetComponent<AudioSource>().isPlaying){
            Destroy(this.gameObject);
        }
    }
}
