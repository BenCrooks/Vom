using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeWorm : MonoBehaviour
{
    [SerializeField] private GameObject explodeEffect;    private float TimeToExplode;
    public void Explode(float killTime = -1)
    {
        TimeToExplode = killTime > -1 ? killTime : Random.Range(0f, 4f);
        StartCoroutine(WaitAndSpawn());
        Destroy(gameObject, TimeToExplode + 0.1f);
    }

    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitForSeconds(TimeToExplode);
        Instantiate(explodeEffect, transform.position, transform.rotation).GetComponent<Emitter>().texture = GetComponent<SpriteRenderer>().sprite.texture;
    }
}
