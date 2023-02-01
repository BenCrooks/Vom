using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public bool isWorm;
    private bool canMove, transforming;
    [SerializeField] private Animator[] animators;
    private float transformationTime, transformationTimer;
    [SerializeField] private AnimationClip transformAnim;
    private GameObject[] oldTerrain,oldTerrain1;
    [SerializeField] private GameObject newTerrain, CoverTerrain;
    private GameObject coverTerrainInstance;
    void Start()
    {

    }

    void FixedUpdate()
    {
        if(transforming){
            if(transformationTimer < transformationTime){
                transformationTimer += Time.deltaTime;
            }else{
                transformationTimer=0;
                transforming=false;
            }
        }else{
            if(!canMove){
                canMove = true;
                if(isWorm){
                    foreach(GameObject oldTer in oldTerrain){
                        Destroy(oldTer);
                    }
                    foreach(GameObject oldTer in oldTerrain1){
                        Destroy(oldTer);
                    }
                    Instantiate(newTerrain,new Vector3(0,0,0),Quaternion.identity);
                    Destroy(coverTerrainInstance);
                    GetComponent<WormController>().enabled = true;
                    GetComponent<WormController>().initiateFirstBlock();
                }else{
                    GetComponent<CharacterController>().enabled = true;
                    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    GetComponent<CapsuleCollider2D>().enabled=true;
                    float extents = GameObject.FindGameObjectWithTag("Terrain").GetComponent<BoxCollider2D>().bounds.extents.y;
                    transform.position = new Vector3(RoundTo(transform.position.x,extents),RoundTo(transform.position.y,extents),transform.position.z);
                }
            }
        }
    }
    static float RoundTo(float value, float multipleOf) {
        return Mathf.Round(value/multipleOf) * multipleOf;
    }

    public void TransformToWorm(){
        if(!isWorm){
            isWorm=true;
            transforming = true;
            canMove=false;
            oldTerrain = GameObject.FindGameObjectsWithTag("Terrain");
            oldTerrain1 = GameObject.FindGameObjectsWithTag("Spawnable");
            coverTerrainInstance = Instantiate(CoverTerrain,new Vector3(0,0,0),Quaternion.identity);
            GetComponent<CharacterController>().enabled = false;
            GetComponent<CapsuleCollider2D>().enabled=false;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            transformationTime = transformAnim.length * 1.1f;
            float terrainSize = GameObject.FindGameObjectWithTag("Terrain").GetComponent<BoxCollider2D>().bounds.extents.y*2;
            Vector3 snapPos =  new Vector3(RoundTo(transform.position.x,terrainSize),RoundTo(transform.position.y,terrainSize),transform.position.z);
            transform.position =snapPos + new Vector3(0,terrainSize+0.15f,0);
            Animate("IsWorm",true);
            for(int i=0; i<animators.Length;i++){
                animators[i].GetComponent<SpriteRenderer>().sortingOrder=4;
            }
            Camera.main.GetComponent<MusicManager>().FadeOutToWormTrack();
        }
    }

    public void TransformToCharacter(){
        if(isWorm){
            isWorm = false;
            transforming = true;
            canMove = false;
            GetComponent<WormController>().enabled = false;
            transformationTime = 1;
            Animate("IsWorm",false);
            for(int i=0; i<animators.Length;i++){
                animators[i].GetComponent<SpriteRenderer>().sortingOrder=1;
            }
            foreach(Transform chld in transform.GetComponentsInChildren<Transform>())
            {
                chld.rotation = Quaternion.Euler(0,0,0);
            }
            Camera.main.GetComponent<MusicManager>().FadeOutToFight();
        }
    }

    private void Animate(string WhatToSet, bool ValueBool){
        for(int i=0; i<animators.Length;i++){
            animators[i].SetBool(WhatToSet,ValueBool);
        }
    }
    public Vector3 FindClosestBlock(Vector3 pos)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Terrain");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - pos;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest.transform.position;
    }

    public void EscButton(){
        Destroy(this.gameObject);
    }
    public void StartButton(){
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().BeginGame();
    }
}
