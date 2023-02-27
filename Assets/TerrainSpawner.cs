using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private float slotSize = 0.36f;
    private bool[] filledTerrainSlots;
    private bool[] filledBackgroundSlots;
    //positioning of slots will work as follows (the same as text)
    //1,2,3,4,5...
    //10,11,12,13..

    [SerializeField] private GameObject backgroundEmpty, background1x1, background2x2, background4x4, background6x6;
    //chance of spawn in %
    [SerializeField] private float chanceOfSpawnEmpty, chanceOfSpawn1x1, chanceOfSpawn2x2, chanceOfSpawn4x4, chanceOfSpawn6x6;
    [SerializeField] private GameObject[] TerrainOneOf, TerrainOrgainic1,TerrainOrgainic2,BoneTerrain,terrainFiller;
    [SerializeField] private GameObject TerrainSpine, cartilageTerrain, cartilageCornerTerrain;
    private LayerMask lyrGround;
    private int numberOfLoopsBeforeFail = 8,numberOfLoopsBeforeFailCounter;
    private int EdgeBufferForTerrain = 6;
    private GameObject skull;

    // Start is called before the first frame update
    void Start()
    {
        lyrGround = LayerMask.GetMask("Terrain");
        filledBackgroundSlots = new bool[width*height];
        filledTerrainSlots = new bool[width*height];
        GenerateBackground();
        GenerateTerrain();
    }

    //____________________general functions____________________
    private Vector2 GetPositionFromArrayPointXY(int x,int y){
        return new Vector2((x-width/2)*slotSize,(height/2-y)*slotSize);
    }
    private Vector2 GetPositionFromArrayUnit(int x){
        int w=0;
        int h=0;
        for(int i =0; i<x; i += width){
            h++;
            if(x-i<width){
                w = x-i;
            }
        }
        return new Vector2((w-width/2)*slotSize,(height/2-h)*slotSize);
    }

    private float Random90DegRotation(){
        return Random.Range( 0, 4 ) * 90;
    }
    private void CheckAllSlotsEmptyAndRepeat(GameObject obj){
        numberOfLoopsBeforeFailCounter++;
        if(numberOfLoopsBeforeFailCounter > numberOfLoopsBeforeFail){
            Destroy(obj);
        }else if(!ReturnCheckEmptySlots(obj)){
            int spawnPos = Random.Range(0,filledTerrainSlots.Length);
            obj.transform.position = GetPositionFromArrayUnit(spawnPos);
            CheckAllSlotsEmptyAndRepeat(obj);
            return;
        }
    }
    private bool ReturnCheckEmptySlots(GameObject obj){
        foreach(Transform child in obj.transform){
            BoxCollider2D col = child.GetComponent<BoxCollider2D>();
            if(col!=null){
                RaycastHit2D[] hit = Physics2D.RaycastAll(child.position, new Vector2(1,1), 0.01f, lyrGround);
                for(int h=0;h<hit.Length;h++){
                    if(hit[h].collider != col && hit[h].collider != null){
                        return false;
                    }
                }
            }
        }
        return true;
    }
    //______________________Functionality____________________

    private void GenerateTerrain(){
        SpawnOneOfTerrain();
        SpawnSpineTerrain();
        SpawnBones();
        // FillRemainingTerrain();
    }

    private void SpawnOneOfTerrain(){
        for(int i=0; i<TerrainOneOf.Length;i++){
            int spawnPos = Random.Range(width+EdgeBufferForTerrain,filledTerrainSlots.Length-EdgeBufferForTerrain-width);
            for(int e=-EdgeBufferForTerrain;e<EdgeBufferForTerrain;e++){
                if((spawnPos + e) % width==0){
                    spawnPos+=e<0?e+EdgeBufferForTerrain:-e-EdgeBufferForTerrain;
                }
            }
            if(filledTerrainSlots[spawnPos] != true){
                Vector2 pos = GetPositionFromArrayUnit(spawnPos);
                GameObject spawnTerrain = Instantiate( TerrainOneOf[i], pos, Quaternion.Euler( 0, Random.Range( 0, 2 ) * 180, Random90DegRotation() ));
                if(i==0){
                    skull = spawnTerrain;
                }
                numberOfLoopsBeforeFailCounter = 0;
                CheckAllSlotsEmptyAndRepeat(spawnTerrain);
            }
        }
    }
    private void SpawnSpineTerrain(){
        bool foundSpace=false;
        int offset=0;
        int offsetDirection = 1;
        int failSafe = 4;
        bool failSafeInvert = false;
        int failSafeCounter = 0;
        bool up = false;
        bool left = false;
        bool horizontal = Random.Range(0, 2) == 1;
        Vector3 skullPos = skull.transform.position;
        if(horizontal){
            if(skullPos.x<0){
                left=true;
            }
        }else{
            if(skullPos.y<0){
                up = true;
            }
        }
        while(foundSpace==false){
            RaycastHit2D[] hit;
            RaycastHit2D[] hit1;
            if(horizontal){
                hit = Physics2D.RaycastAll(skullPos + new Vector3(0,-slotSize/2+offset*offsetDirection*slotSize,0), new Vector2(left?1:-1,0), width*slotSize, lyrGround);
                hit1 = Physics2D.RaycastAll(skullPos + new Vector3(0,slotSize/2+offset*offsetDirection*slotSize,0), new Vector2(left?1:-1,0), width*slotSize, lyrGround);
                // Debug.DrawRay(skullPos + new Vector3(0,-slotSize/2+offset*offsetDirection*slotSize,0),new Vector2(left?1:-1,0),Color.red);
                // Debug.DrawRay(skullPos + new Vector3(0,slotSize/2+offset*offsetDirection*slotSize,0),new Vector2(left?1:-1,0),Color.red);
                // Debug.DrawLine(skullPos + new Vector3(0,-slotSize/2+offset*offsetDirection*slotSize,0),skullPos + new Vector3(0,slotSize/2+offset*offsetDirection*slotSize,0));
            }else{
                hit = Physics2D.RaycastAll(skullPos+ new Vector3(-slotSize/2+offset*offsetDirection*slotSize,0,0), new Vector2(0,up?1:-1), height*slotSize, lyrGround);
                hit1 = Physics2D.RaycastAll(skullPos+ new Vector3(slotSize/2+offset*offsetDirection*slotSize,0,0), new Vector2(0,up?1:-1), height*slotSize, lyrGround);
                // Debug.DrawRay(skullPos+ new Vector3(-slotSize/2+offset*offsetDirection*slotSize,0,0),new Vector2(0,up?1:-1),Color.red);
                // Debug.DrawRay(skullPos+ new Vector3(slotSize/2+offset*offsetDirection*slotSize,0,0),new Vector2(0,up?1:-1),Color.red);
                // Debug.DrawLine(skullPos+ new Vector3(-slotSize/2+offset*offsetDirection*slotSize,0,0),skullPos+ new Vector3(slotSize/2+offset*offsetDirection*slotSize,0,0));
            }
            bool tempFoundSpace = true;
            for(int i=0;i<hit.Length;i++){
                if(hit[i].collider!=null){
                    if(hit[i].collider.transform.parent.gameObject!=skull){
                        tempFoundSpace = false;
                        if(offset==0){
                            offsetDirection = 1;
                        }
                    }
                }
            }
            for(int i=0;i<hit1.Length;i++){
                if(hit1[i].collider!=null){
                    if(hit1[i].collider.transform.parent.gameObject!=skull){
                        print(hit1[i].collider.transform.parent.gameObject.name);
                        tempFoundSpace = false;
                        if(offset==0){
                            offsetDirection = -1;
                        }
                    }
                }
            }
            if(tempFoundSpace == true){
                foundSpace = true;
                for(int s=0;s<(horizontal?width:height);s++){
                    float relativePos = s*slotSize;
                    if((!horizontal&&!up)||(horizontal&&!left)){
                        relativePos = -relativePos;
                    }
                    Vector3 pos1 = horizontal ? new Vector3(relativePos,0,0) + skullPos + new Vector3(0,-slotSize/2+offset*offsetDirection*slotSize,0) : new Vector3(0,relativePos,0) + skullPos+ new Vector3(-slotSize/2+offset*offsetDirection*slotSize,0,0);
                    Vector3 pos2 = horizontal ? new Vector3(relativePos,0,0) + skullPos + new Vector3(0,slotSize/2+offset*offsetDirection*slotSize,0) : new Vector3(0,relativePos,0) + skullPos+ new Vector3(slotSize/2+offset*offsetDirection*slotSize,0,0);
                    Vector3 spawnPos = (pos1+pos2)/2;
                    if(spawnPos.y<height*slotSize/2 && spawnPos.y>-height*slotSize/2 && spawnPos.x<width*slotSize/2 && spawnPos.x>-width*slotSize/2)
                    {
                        RaycastHit2D hit3 = Physics2D.Raycast( pos1+new Vector3(horizontal?left?-1:1:0,horizontal?0:up?-1:1)*0.1f, new Vector2(horizontal?left?1:-1:0,horizontal?0:up?-1:1), 0.1f, lyrGround);
                        RaycastHit2D hit4 = Physics2D.Raycast( pos2+new Vector3(horizontal?left?-1:1:0,horizontal?0:up?-1:1)*0.1f, new Vector2(horizontal?left?1:-1:0,horizontal?0:up?-1:1), 0.1f, lyrGround);
                        // Debug.DrawLine(pos1+(new Vector3(horizontal?left?-1:1:0,horizontal?0:up?-1:1)*0.1f),pos1+(new Vector3(horizontal?left?1:-1:0,horizontal?0:up?-1:1)*0.2f));
                        // Debug.DrawLine(pos2+(new Vector3(horizontal?left?-1:1:0,horizontal?0:up?-1:1)*0.1f),pos2+(new Vector3(horizontal?left?1:-1:0,horizontal?0:up?-1:1)*0.2f));
                        if(hit3.collider==null&&hit4.collider==null){
                            Instantiate(TerrainSpine,spawnPos,Quaternion.Euler(0,0,horizontal?left?90:-90:up?180:0));
                        }
                    }
                }
            }else{
                failSafeCounter++;
                if(failSafeCounter>failSafe){
                    if(failSafeInvert){
                        foundSpace = true;
                        print("failed to spawn spine");
                    }
                    else{
                        failSafeInvert=true;
                        offsetDirection = offsetDirection*-1;
                        offset = offsetDirection;
                        failSafeCounter=0;
                    }
                }
                offset+= offsetDirection;
            }
        }
    }

    private void SpawnBones(){
        for(int a=0;a<3;a++){
            float randomRotationStart = Random90DegRotation();
            int spawnPos = Random.Range(width+EdgeBufferForTerrain*2,filledTerrainSlots.Length-EdgeBufferForTerrain*2-width);
            int randomBone = Random.Range(0,BoneTerrain.Length-1);
            GameObject chosenBone = Instantiate(BoneTerrain[randomBone],GetPositionFromArrayUnit(spawnPos),Quaternion.Euler(0,0,randomRotationStart));
            CheckAllSlotsEmptyAndRepeat(chosenBone);
            
            Transform parTrans = chosenBone.transform;
            for(int j=0;j<Random.Range(1,3);j++){
                bool foundRotation = false;
                int itteration = 0;
                randomBone = Random.Range(0,BoneTerrain.Length-1);
                randomRotationStart = Random90DegRotation();
                Vector3 spawnPosNu = SetBoneOffset(parTrans, randomRotationStart);
                if(spawnPosNu.y<height*slotSize/2 && spawnPosNu.y>-height*slotSize/2 && spawnPosNu.x<width*slotSize/2 && spawnPosNu.x>-width*slotSize/2)
                {
                    GameObject nuBone = Instantiate(BoneTerrain[randomBone],spawnPosNu,Quaternion.Euler(0,0,randomRotationStart));
                    while(!foundRotation){
                        itteration++;
                        if(itteration>=4){
                            foundRotation = true;
                            Destroy(nuBone);
                            j = 100;
                        }else{
                            float angleDif = DifferenceInAngle(parTrans.eulerAngles.z,nuBone.transform.eulerAngles.z);
                            // if(angleDif > 20){
                                foundRotation = ReturnCheckEmptySlots(nuBone); 
                            // }
                            if(!foundRotation){
                                nuBone.transform.Rotate(0,0,90);
                                float zRot = nuBone.transform.eulerAngles.z;
                                spawnPosNu = SetBoneOffset(parTrans, zRot);
                            }else{
                                Vector3 GoopPos = (parTrans.position + (parTrans.up * (parTrans.childCount + 1) * slotSize));
                                bool corner = false;
                                bool invert = false;
                                bool offset = false;
                                if(angleDif<100 && angleDif>80){
                                    corner = true;
                                }else if(angleDif<280 && angleDif>260){
                                    corner = true;
                                    invert = true;
                                }else if(angleDif<190 && angleDif>170){
                                    corner = true;
                                    offset = true;
                                }
                                Instantiate(!corner?cartilageTerrain:cartilageCornerTerrain, GoopPos + nuBone.transform.right*(offset?-slotSize:0), Quaternion.Euler(nuBone.transform.eulerAngles + new Vector3(0,0,invert?180:0)));
                                parTrans = nuBone.transform;
                            }
                        }
                    }
                }
            }
        }
    }
    private float DifferenceInAngle (float Angle1, float Angle2){
        float diffInAngle = Angle1 - Angle2;
        while(diffInAngle<0){
            diffInAngle += 360;
        }
        while(diffInAngle >= 360){
            diffInAngle -= 360;
        }
        return diffInAngle;
    }

    private Vector3 SetBoneOffset(Transform ParentTrans, float zAngle ){
        float diffInAngle = DifferenceInAngle(ParentTrans.eulerAngles.z, zAngle);
        Vector3 angleOffset = default(Vector3);
        if(diffInAngle < 110 && diffInAngle > 80){
            //right
            angleOffset = ParentTrans.right * slotSize;
        }else if(diffInAngle < 280 && diffInAngle > 260){
            //left
            angleOffset = ParentTrans.up * -slotSize ;//+ ParentTrans.right * -slotSize*2;
        }
        Vector3 spawnPosNu = (ParentTrans.position + (ParentTrans.up * (ParentTrans.childCount + 1) * slotSize) + angleOffset);
        return spawnPosNu;
    }

    private void FillRemainingTerrain(){
        for(int i=0; i<filledTerrainSlots.Length;i++){
            if(filledTerrainSlots[i]==false){
                Vector2 pos = GetPositionFromArrayUnit(i);
                Instantiate(terrainFiller[Random.Range(0,terrainFiller.Length)],pos,Quaternion.identity);
            }
        }
    }
    private void GenerateBackground(){
        for(int i =1; i<width;i++)
        {   
            for(int j =1; j<height;j++)  
            {
                if(filledBackgroundSlots[i*height+j] != true){
                    int spaceTaken = 0;
                    Vector2 pos = GetPositionFromArrayPointXY(i,j);
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
}
