using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] song1, song2;
    AudioSource audioSource;
    private float fadeTimer,FadeTime=1;
    private bool fade=false;
    private AudioClip nextTrack;
    private float MaxVol;
    private int lastPlayed=0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        MaxVol = audioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if(fade){
            fadeTimer+=Time.deltaTime;
            audioSource.volume = MaxVol*(FadeTime-fadeTimer/FadeTime);
            if(fadeTimer>FadeTime){
                fadeTimer=0;
                fade=false;
                audioSource.clip=nextTrack;
                audioSource.volume=MaxVol;
                audioSource.Play();
            }
        }
    }
    public void FadeOutToWormTrack(){
        fade=true;
        nextTrack = song2[lastPlayed];
    }
    public void FadeOutToFight(){
        lastPlayed++;
        if(lastPlayed>=song1.Length){
            lastPlayed=0;
        }
        fade=true;
        nextTrack = song1[lastPlayed];
    }
}
