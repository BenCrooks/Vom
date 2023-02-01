using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkAndHitGround : MonoBehaviour
{
    private GameObject par;
    private void Start() {
        par = transform.parent.gameObject;
    }
    private void OnCollisionEnter2D(Collision2D col) {
        if(col.collider != null){
            if(col.gameObject.tag == "Terrain" || col.gameObject.tag == "Spawnable"){
                IDamageable<float,GameObject> damageable = col.gameObject.GetComponent<IDamageable<float, GameObject>>();
                if(damageable != null){
                    damageable.Damage(par.GetComponent<PlayerBullet>().damage,par.GetComponent<PlayerBullet>().Shooter);
                }
                if(col.gameObject.GetComponent<Rigidbody2D>()!=null){
                    if(col.gameObject.GetComponent<Rigidbody2D>().bodyType!= RigidbodyType2D.Static)
                    col.gameObject.GetComponent<Rigidbody2D>().velocity=new Vector2(0,0);
                }
                par.transform.parent = col.transform;
                Destroy(par.GetComponent<Rigidbody2D>());
                transform.position = col.contacts[0].point;
                // par.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                Destroy(par.gameObject,0.4f);
            }
        }
    }
}
