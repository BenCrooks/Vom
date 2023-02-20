using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private float slotSize = 0.36f;
    private bool[] filledBackgroundSlots;
    //positioning of slots will work as follows (the same as text)
    //1,2,3,4,5...
    //10,11,12,13..

    [SerializeField] private GameObject backgroundEmpty, background1x1, background2x2, background4x4, background6x6;
    //chance of spawn in %
    [SerializeField] private float chanceOfSpawnEmpty, chanceOfSpawn1x1, chanceOfSpawn2x2, chanceOfSpawn4x4, chanceOfSpawn6x6;
    [SerializeField] private bool[] canSpawnMoreThanOne;
    [SerializeField] private float[] chanceOfSpawning;
    [SerializeField] private GameObject[] Terrain;
    private LayerMask lyrGround;

    // Start is called before the first frame update
    void Start()
    {
        lyrGround = LayerMask.NameToLayer("Terrain");
        filledBackgroundSlots = new bool[width*height];
        GenerateBackground();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateBackground(){
        for(int i =1; i<width;i++)
        {   
            for(int j =1; j<height;j++)  
            {
                if(filledBackgroundSlots[i*height+j] != true){
                    int spaceTaken = 0;
                    Vector2 pos = new Vector2((i-width/2)*slotSize,(height/2-j)*slotSize);
                    float whatToSpawn = Random.Range(0,chanceOfSpawnEmpty+ chanceOfSpawn1x1+ chanceOfSpawn2x2+ chanceOfSpawn4x4+ chanceOfSpawn6x6);
                    
                    //first check if you can spawnBig blocks
                    int canSpawnSize=6;
                    for(int s=1; s<=6;s++){
                        if(i*height+(j+s)<filledBackgroundSlots.Length){
                            if(filledBackgroundSlots[i*height+(j+s)] == true)
                            {
                                canSpawnSize = s;
                                s=1000;
                            }
                        }
                    }
                    
                    if(whatToSpawn <= chanceOfSpawn6x6 && canSpawnSize==6){
                        Instantiate(background6x6, pos ,Quaternion.identity).transform.parent = gameObject.transform;
                        spaceTaken = 6;
                    }else if(whatToSpawn <= chanceOfSpawn6x6 + chanceOfSpawn4x4 && canSpawnSize>=4){
                        Instantiate(background4x4, pos ,Quaternion.identity).transform.parent = gameObject.transform;
                        spaceTaken = 4;
                    }else if(whatToSpawn <= chanceOfSpawn6x6 + chanceOfSpawn4x4 + chanceOfSpawn2x2 && canSpawnSize>=2){
                        Instantiate(background2x2, pos ,Quaternion.identity).transform.parent = gameObject.transform;
                        spaceTaken = 2;
                    }else if(whatToSpawn <= chanceOfSpawn1x1 + chanceOfSpawn2x2 + chanceOfSpawn4x4 + chanceOfSpawn6x6){
                        Instantiate(background1x1, pos ,Quaternion.identity).transform.parent = gameObject.transform;
                    }else if(whatToSpawn <=chanceOfSpawnEmpty + chanceOfSpawn1x1 + chanceOfSpawn2x2 + chanceOfSpawn4x4 + chanceOfSpawn6x6){
                        Instantiate(backgroundEmpty, pos ,Quaternion.identity).transform.parent = gameObject.transform;
                    }else{
                        print("chance to spawn of background needs to total 100 (as a percent: Terrain Spawner): "+ whatToSpawn);
                    }

                    for(int x=0; x<spaceTaken; x++){
                        for(int y=0; y<spaceTaken; y++){
                            if((i+x)*(j+y)<filledBackgroundSlots.Length){
                                if(j+y<height){
                                    if((i+x)*height+(j+y)<filledBackgroundSlots.Length)
                                    filledBackgroundSlots[(i+x)*height+(j+y)] = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void GenerateTerrain(){
        float chanceSpawnTotal=0;
        for(int c =0; c< chanceOfSpawning.Length;c++){
            chanceSpawnTotal+=chanceOfSpawning[c];
        }
        for(int i =1; i<width;i++)
        {   
            for(int j =1; j<height;j++)  
            {
                Vector2 pos = new Vector2((i-width/2)*slotSize,(height/2-j)*slotSize);
                RaycastHit2D hitOrigin = Physics2D.Raycast(pos, -Vector2.up,0.1f, lyrGround);
                if(hitOrigin.collider==null){
                    bool objInTheWay = false;
                    int RandomTerrainObj = -1;
                    float tempChanceToSpawnCounter=0;
                    float percentChanceTerItem = Random.Range(0,chanceSpawnTotal);
                    for(int c =0; c< chanceOfSpawning.Length;c++){
                        tempChanceToSpawnCounter += chanceOfSpawning[c];
                        if(tempChanceToSpawnCounter > percentChanceTerItem){
                            RandomTerrainObj = c-1;
                        }
                    }
                    GameObject spawnTerrain = Terrain[RandomTerrainObj];
                    foreach(Transform child in spawnTerrain.transform){
                        RaycastHit2D hit = Physics2D.Raycast(child.position, -Vector2.up,0.1f, lyrGround);
                        if(hit.collider!=null){
                            objInTheWay = true;
                        }
                    }
                    if(!objInTheWay){
                        Instantiate(spawnTerrain,pos,Quaternion.identity);
                    }else{
                        j-=1;
                    }
                }
            }
        }
    }
}
