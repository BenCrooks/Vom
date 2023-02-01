using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class managePlayerAudio : MonoBehaviour
{
    private AudioSource AS;
    [SerializeField] private AudioClip footstepSound, slideSound;
    [SerializeField] Sprite[] footstepSprites, slideSoundSprites;
    private Sprite lastSprite;
    private SpriteRenderer sprRndr;

    // Start is called before the first frame update
    void Start()
    {
        AS=GetComponent<AudioSource>();
        sprRndr = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Sprite spr in footstepSprites){
            if(sprRndr.sprite!=lastSprite){
                if(sprRndr.sprite == spr){
                    lastSprite=spr;
                    SetAndPlaySound(footstepSound,0.3f,0,0.7f);
                }
            }
        }
        foreach(Sprite spr in slideSoundSprites){
            if(sprRndr.sprite!=lastSprite){
                if(sprRndr.sprite == spr){
                    lastSprite=spr;
                    SetAndPlaySound(slideSound,0.4f,-0.5f,0.8f);
                }
            }
        }
        // for(int i=0; i<slideSoundSprites.Length;i++){
        //     bool sliding=false;
        //     if(sprRndr.sprite!=lastSprite){
        //         if(sprRndr.sprite == slideSoundSprites[i]){
        //             lastSprite = slideSoundSprites[i];
        //             SetAndPlaySound(slideSound,0.1f);
        //         }
        //     }
        // }
    }
    public void SetAndPlaySound(AudioClip clip, float pitchRandomizer, float pitchOffset, float volume){
        if(GetComponent<CharacterController>().isActiveAndEnabled){
            AS.clip = clip;
            AS.pitch = 1 + Random.Range(-pitchRandomizer,pitchRandomizer)+pitchOffset;
            AS.volume = volume;
            AS.Play();
        }
    }
}
