using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterPlayback : MonoBehaviour
{
    private AudioSource src;
    private void Start() {
        src = GetComponent<AudioSource>();
    }
    public void DestroyAfterPlay(){
        transform.parent = null;
        StartCoroutine(WaitTillCompleteThenDestroy());
    }
    private IEnumerator WaitTillCompleteThenDestroy(){
        yield return new WaitForSeconds(src.clip.length);
        this.gameObject.SetActive(false);
    }
}
