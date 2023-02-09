using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] public float damage=1;
    public Vector2 DirectionForce;
    private GameObject Cam;
    public GameObject Shooter;
    [SerializeField] private float LifeTime;
    private float LifeTimer;
    [SerializeField] private float maxSpeed;
    [HideInInspector] public Color ParticleCol;
    private GameObject ScreenShakeManager;
    [SerializeField]private GameObject quickPlayAudio;
    [SerializeField] private AudioClip[] clip;
    [SerializeField] private float screenShakeTime = 0.19f;
    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddForce(DirectionForce);
        Cam = Camera.main.gameObject;
        transform.Find("Particle System").GetComponent<ParticleSystem>().startColor = ParticleCol;
        if(ScreenShakeManager==null){
            ScreenShakeManager = GameObject.Find("ScreenShakeManager");
        }
    }
    private void Update() {
        LifeTimer+=Time.deltaTime;
        if(LifeTimer> LifeTime){
            Destroy(this.gameObject);
        }
    if (rb2d != null){
            float speed = Vector3.Magnitude (rb2d.velocity);  // test current object speed
            if(speed > maxSpeed){
                float brakeSpeed = speed - maxSpeed;  // calculate the speed decrease
                Vector3 normalisedVelocity = rb2d.velocity.normalized;
                Vector3 brakeVelocity = normalisedVelocity * brakeSpeed;  // make the brake Vector3 value
                rb2d.AddForce(-brakeVelocity);  // apply opposing brake force
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col != null){
            if(col.gameObject!=Shooter){
                IDamageable<float,GameObject> damageable = col.gameObject.GetComponent<IDamageable<float, GameObject>>();
                if(damageable != null){
                    damageable.Damage(damage,Shooter);
                    if(col.gameObject.tag == "Player"){
                        if(ScreenShakeManager!=null){
                            if(rb2d!=null){
                                Vector2 vel = rb2d.velocity.normalized;
                                ScreenShakeManager.GetComponent<ManageScreenShakeObjects>().ShakeAll(screenShakeTime,vel);
                            }
                        }
                        Destroy(gameObject);
                    }else{
                        if(quickPlayAudio!=null){
                            GameObject qp = Instantiate(quickPlayAudio,transform.position,Quaternion.identity);
                            qp.GetComponent<PlayAudioAndDelete>().clip = clip[Random.Range(0,clip.Length-1)];
                            qp.GetComponent<AudioSource>().pitch += Random.Range(-0.3f,0.3f);
                            qp.GetComponent<AudioSource>().volume += Random.Range(-0.7f,-0.4f);
                        }
                    }
                }
            }
        }
    }
}
