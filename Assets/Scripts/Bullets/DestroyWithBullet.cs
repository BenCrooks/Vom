using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWithBullet : MonoBehaviour
{
    [SerializeField] private GameObject explodeEffect;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Bullet"){
            GameObject efect = Instantiate(explodeEffect,transform.position,transform.rotation);
            // efect.GetComponent<ParticleSystem>().shape = 
            Destroy(this.gameObject);
        }
    }
}
