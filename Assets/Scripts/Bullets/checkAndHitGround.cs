using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkAndHitGround : MonoBehaviour
{
    private GameObject par;
    private PlayerBullet playerBullet;
    private void Start() {
        par = transform.parent.gameObject;
        playerBullet = par.GetComponent<PlayerBullet>();
    }
    private void OnCollisionEnter2D(Collision2D col) {
        if(col.collider != null){
            if(col.gameObject.tag == "Terrain" || col.gameObject.tag == "Spawnable"){
                IDamageable<float,GameObject> damageable = col.gameObject.GetComponent<IDamageable<float, GameObject>>();
                if(damageable != null){
                    damageable.Damage(playerBullet.damage,playerBullet.Shooter);
                }
                Rigidbody2D rigidbody = col.gameObject.GetComponent<Rigidbody2D>();
                if(rigidbody != null && rigidbody.bodyType != RigidbodyType2D.Static){
                    rigidbody.velocity = Vector2.zero;
                }
                par.transform.parent = col.transform;
                Destroy(par.GetComponent<Rigidbody2D>());
                transform.position = col.contacts[0].point;
                Destroy(par.gameObject,0.4f);
            }
        }
    }
}
