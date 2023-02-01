using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeWorm : MonoBehaviour
{
    [SerializeField] private GameObject explodeEffect;
    private float TimeToExplode;
    // Start is called before the first frame update
    void Start()
    {
        TimeToExplode = Random.Range(0f,2f)+Random.Range(0f,2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Explode(float killTime = -1){
        if(killTime == -1){
            killTime = TimeToExplode;
        }else{
            TimeToExplode=killTime;
        }
        StartCoroutine(WaitAndSpawn());
        Destroy(this.gameObject,killTime+0.1f);
    }
    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitForSeconds(TimeToExplode);
        GameObject efect = Instantiate(explodeEffect,transform.position,transform.rotation);
        efect.GetComponent<Emitter>().texture = GetComponent<SpriteRenderer>().sprite.texture;
    }
}
