using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthObj : MonoBehaviour, IDamageable<float,GameObject>,IKillable
{
    public float Health;
    [SerializeField] private GameObject quickPlayAudio;
    [SerializeField] private AudioClip clip;
    private bool spawnedAudio =false;

    public void Kill(GameObject whatKilledMe){
        if(quickPlayAudio!=null && !spawnedAudio){
            spawnedAudio=true;
            GameObject qp = Instantiate(quickPlayAudio,transform.position,Quaternion.identity);
            qp.GetComponent<PlayAudioAndDelete>().clip = clip;
            qp.GetComponent<AudioSource>().pitch += Random.Range(-0.15f,0.15f);
        }
        if(GetComponent<ExplodeWorm>()!=null){
            GetComponent<ExplodeWorm>().Explode(0);
        }else{
            Destroy(this.gameObject);
        }
    }
    public void Damage(float damageTaken, GameObject go){
        Health -= damageTaken;
        if(Health<=0){
            Kill(go);
        }
    }
}
