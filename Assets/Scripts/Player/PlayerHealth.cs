using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable<float,GameObject>,IKillable
{
    public float Health;
    public float startHealth;
    private Rigidbody2D rb;
    private Camera cam;
    public bool dead=false;
    [SerializeField] public SpriteRenderer[] sprites;
    [SerializeField] private float knockbackForce=1000, KnockUpForce=300;
    [SerializeField] private GameObject particlesDeath;
    [SerializeField] private float ImmuneTime = 0.5f;
    private float ImmuneTimer;
    public bool immune;
    private Material ownMaterial;
    [SerializeField] private AudioSource AudioSrc;
    [SerializeField] private AudioClip HurtSound, DieSound;
    [HideInInspector] public GameObject HealthUI;
    [SerializeField] private float flashBrightness=8;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        startHealth = Health;
        ownMaterial = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
        foreach(SpriteRenderer spr in sprites){
            ownMaterial.SetFloat("Flash Brightness", flashBrightness);
        }
    }
    private void Update() {
        if(immune){
            ImmuneTimer+=Time.deltaTime;
            if(ImmuneTimer>=ImmuneTime){
                ImmuneTimer=0;
                immune=false;
            }
        }
        // var pos = cam.WorldToScreenPoint(transform.position);
        // Vector2 screenSize = new Vector2((float)Screen.width, (float)Screen.height);
        // if(pos.y<0)
        // {
        //     Damage(10, this.gameObject);
        //     if(Health-10>0){
        //         rb.velocity= new Vector2(0,0);
        //         rb.AddForce(Vector3.up*90000);
        //     }
        // }
        // if(pos.y>Screen.height){
        //     Damage(10, this.gameObject);
        //     if(Health-10 > 0){
        //         rb.velocity= new Vector2(0,0);
        //         rb.AddForce(-Vector3.up*50000);
        //     }
        // }
    }

    public void Kill(GameObject hitMe){
        GameObject Respawner = GameObject.FindWithTag("Respawn");
        if(Respawner!=null){
            transform.position = Respawner.transform.position;
            GetComponent<CharacterController>().CanMove = false;
            GetComponent<AimController>().CanShoot = false;
        }else{
            dead = true;
            GetComponent<AimController>().CanShoot = false;
            // GetComponent<CharacterController>().CanMove = false;
            GetComponent<CharacterController>().DeathHide();
            Instantiate(particlesDeath,transform.position, Quaternion.identity);
            GameObject pm = GameObject.Find("PlayerManager");
            pm.GetComponent<PlayerManager>().InGamePlayerCount -= 1;
            if(pm.GetComponent<PlayerManager>().InGamePlayerCount == 1){
                pm.GetComponent<PlayerManager>().VictoriousPlayer = hitMe;
            } 
        }
    }
    public void ResetPlayerHealth(){
        HealthUI.GetComponent<WatchPlayersHealth>().UpdateHealthUI(6);
        GetComponent<AimController>().enabled=true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        foreach(SpriteRenderer spr in sprites){
            spr.gameObject.SetActive(true);
            transform.localScale=new Vector3(1,1,1);
            spr.material = ownMaterial;
            Health=startHealth;
            dead=false;
            AudioSrc.gameObject.SetActive(true);
        }
    }
    
    public async void Damage(float damageTaken, GameObject go){
        if(!dead && !immune){
            immune=true;
            Vector3 angle = (transform.position - go.transform.position).normalized;
            //reduce health
            Health -= damageTaken;
            HealthUI.GetComponent<WatchPlayersHealth>().UpdateHealthUI((int)Health);
            if(Health<=0){
                SetAndPlaySound(DieSound,0f);
                AudioSrc.gameObject.GetComponent<DestroyAfterPlayback>().DestroyAfterPlay();
                Kill(go);
            }else{
                StartCoroutine(damageFlash());
                //knockback
                rb.AddForce(angle*knockbackForce);
                rb.AddForce(Vector3.up*KnockUpForce);
                SetAndPlaySound(HurtSound,0.3f);
            }
        }
    }

    IEnumerator damageFlash(){
        foreach(SpriteRenderer spr in sprites){
            ownMaterial.SetFloat("_Flashing", 1);
        }
        yield return new WaitForSeconds(ImmuneTime*0.5f);
        foreach(SpriteRenderer spr in sprites){
            ownMaterial.SetFloat("_Flashing", 0);
        }
        yield return new WaitForSeconds(ImmuneTime*0.55f);
        foreach(SpriteRenderer spr in sprites){
            ownMaterial.SetFloat("_Flashing", 1);
        }
        yield return new WaitForSeconds(ImmuneTime*0.85f);
        foreach(SpriteRenderer spr in sprites){
            ownMaterial.SetFloat("_Flashing", 0);
        }
        yield return new WaitForSeconds(ImmuneTime*0.9f);
        foreach(SpriteRenderer spr in sprites){
            ownMaterial.SetFloat("_Flashing", 1);
        }
        yield return new WaitForSeconds(ImmuneTime);
        foreach(SpriteRenderer spr in sprites){
            ownMaterial.SetFloat("_Flashing", 0);
        }
    }

    private void SetAndPlaySound(AudioClip clip, float pitchRandomizer){
        AudioSrc.clip = clip;
        AudioSrc.pitch = 1 + Random.Range(-pitchRandomizer,pitchRandomizer);
        AudioSrc.Play();
    }

}
