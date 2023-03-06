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
    [SerializeField] private GameObject[] TerrainOneOf, TerrainIntestine1Corner,TerrainIntestine1Straight,TerrainIntestine2Corner, TerrainIntestine2Straight,BoneTerrain,terrainFiller;
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
        StartCoroutine(GenerateTerrain());
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

    private int AccountForHorizontalEdge(int pos){
        for(int e=-EdgeBufferForTerrain;e<EdgeBufferForTerrain;e++){
            if((pos + e) % width==0){
                pos+=e<0?e+EdgeBufferForTerrain:-e-EdgeBufferForTerrain;
            }
        }
        return pos;
    }
    //______________________Functionality____________________

    private IEnumerator GenerateTerrain(){
        SpawnOneOfTerrain();
        yield return new WaitForSeconds(0);
        SpawnSpineTerrain();
        yield return new WaitForSeconds(0);
        SpawnBones();
        yield return new WaitForSeconds(0);
        SpawnIntestine(TerrainIntestine1Corner,TerrainIntestine1Straight);
        yield return new WaitForSeconds(0);
        SpawnIntestine(TerrainIntestine2Corner,TerrainIntestine2Straight);
        yield return new WaitForSeconds(0);
        FillRemainingTerrain();
    }

    private void SpawnOneOfTerrain(){
        for(int i=0; i<TerrainOneOf.Length;i++){
            int spawnPos = Random.Range(width+EdgeBufferForTerrain,filledTerrainSlots.Length-EdgeBufferForTerrain-width);
            spawnPos = AccountForHorizontalEdge(spawnPos);
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
            }else{
                hit = Physics2D.RaycastAll(skullPos+ new Vector3(-slotSize/2+offset*offsetDirection*slotSize,0,0), new Vector2(0,up?1:-1), height*slotSize, lyrGround);
                hit1 = Physics2D.RaycastAll(skullPos+ new Vector3(slotSize/2+offset*offsetDirection*slotSize,0,0), new Vector2(0,up?1:-1), height*slotSize, lyrGround);
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
                        if(hit3.collider==null && hit4.collider==null){
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

    private void SpawnIntestine(GameObject[] IntestineCorners, GameObject[] IntestineStraight){
        int StartPos = Random.Range(width+EdgeBufferForTerrain,filledTerrainSlots.Length-EdgeBufferForTerrain-width);
        StartPos = AccountForHorizontalEdge(StartPos);
        Vector2 pos = GetPositionFromArrayUnit(StartPos);

        RaycastHit2D rayInitial1 = Physics2D.Raycast(pos + new Vector2(slotSize/2,slotSize/2), Vector2.up, 0.1f,lyrGround);
        RaycastHit2D rayInitial2 = Physics2D.Raycast(pos + new Vector2(slotSize/2,-slotSize/2), Vector2.up, 0.1f,lyrGround);
        RaycastHit2D rayInitial3 = Physics2D.Raycast(pos + new Vector2(-slotSize/2,-slotSize/2), Vector2.up, 0.1f,lyrGround);
        RaycastHit2D rayInitial4 = Physics2D.Raycast(pos + new Vector2(-slotSize/2,slotSize/2), Vector2.up, 0.1f,lyrGround);
        bool initailSetup = true;
        int InitialFailSafe = 30;
        int InitialFailSafeCounter =0;
        while(initailSetup&&(rayInitial1.collider!=null||rayInitial2.collider!=null||rayInitial3.collider!=null||rayInitial4.collider!=null)){
            InitialFailSafeCounter++;
            if(InitialFailSafeCounter>InitialFailSafe){
                initailSetup = false;
            }
            StartPos = Random.Range(width+EdgeBufferForTerrain,filledTerrainSlots.Length-EdgeBufferForTerrain-width);
            StartPos = AccountForHorizontalEdge(StartPos);
            pos = GetPositionFromArrayUnit(StartPos);
            rayInitial1 = Physics2D.Raycast(pos + new Vector2(slotSize/2,slotSize/2), Vector2.up, 0.1f,lyrGround);
            rayInitial2 = Physics2D.Raycast(pos + new Vector2(slotSize/2,-slotSize/2), Vector2.up, 0.1f,lyrGround);
            rayInitial3 = Physics2D.Raycast(pos + new Vector2(-slotSize/2,-slotSize/2), Vector2.up, 0.1f,lyrGround);
            rayInitial4 = Physics2D.Raycast(pos + new Vector2(-slotSize/2,slotSize/2), Vector2.up, 0.1f,lyrGround);
        }
        int length = Random.Range(30,50);
        int spawnNumber = 4;
        Vector3 startIntestine1 = pos + new Vector2(slotSize/2,slotSize/2);
        Vector3 startIntestine2 = pos + new Vector2(slotSize/2,-slotSize/2);
        Vector3 startIntestine3 = pos + new Vector2(-slotSize/2,-slotSize/2);
        Vector3 startIntestine4 = pos + new Vector2(-slotSize/2,slotSize/2);
        List<Vector3> allIntestines = new List<Vector3>();
        allIntestines.Add(startIntestine1);
        allIntestines.Add(startIntestine2);
        allIntestines.Add(startIntestine3);
        allIntestines.Add(startIntestine4);
        bool doneSpawning = false;
        int failsafe =30;
        int failSafeCounter =0;
        while (!doneSpawning)
        {   
            int randomIntestine1 = Random.Range(0,allIntestines.Count-1);
            int randomIntestine2 = randomIntestine1++;
            failSafeCounter++;
            if(failSafeCounter>failsafe){
                doneSpawning = true;
            }
            //get the two intestine pieces
            if(randomIntestine2>allIntestines.Count-1){
                randomIntestine2 = 0;
            }
            //ray perpendicular
            bool horizontal = false;
            if(allIntestines[randomIntestine1].x - allIntestines[randomIntestine2].x > 0){
                horizontal = false;
            }else if(allIntestines[randomIntestine1].y - allIntestines[randomIntestine2].y > 0){
                horizontal = true;
            }
            bool canExpand = false;
            bool invert = false;
            Vector3 nextPos1 = allIntestines[randomIntestine1] + new Vector3(horizontal?slotSize:0,horizontal?0:slotSize,0);
            Vector3 nextPos2 = allIntestines[randomIntestine2] + new Vector3(horizontal?slotSize:0,horizontal?0:slotSize,0);
            RaycastHit2D ray = Physics2D.Raycast( nextPos1, horizontal?Vector2.right:Vector2.up, 0.1f, lyrGround);
            RaycastHit2D ray1 = Physics2D.Raycast( nextPos2, horizontal?Vector2.right:Vector2.up, 0.1f, lyrGround);

            //if empty spawn and expand into that area
            if(ray.collider == null && ray1.collider == null && !EqualsV3(allIntestines,nextPos1) && !EqualsV3(allIntestines,nextPos2) && (Mathf.Abs(nextPos1.x)<(width-4)*slotSize/2 && Mathf.Abs(nextPos1.y)<(height-4)*slotSize/2) && (Mathf.Abs(nextPos2.x)<(width-4)*slotSize/2 && Mathf.Abs(nextPos2.y)<(height-4)*slotSize/2)){
                canExpand = true;
            }else{
                Vector3 nextPos3 = allIntestines[randomIntestine1] + new Vector3(horizontal?-slotSize:0,horizontal?0:-slotSize,0);
                Vector3 nextPos4 = allIntestines[randomIntestine2] + new Vector3(horizontal?-slotSize:0,horizontal?0:-slotSize,0);
                if(!EqualsV3(allIntestines,nextPos3) && ! EqualsV3(allIntestines,nextPos4) && (Mathf.Abs(nextPos3.x)<(width-4)*slotSize/2 && Mathf.Abs(nextPos3.y)<(height-4) * slotSize/2) && (Mathf.Abs(nextPos4.x)<(width-4)*slotSize/2 && Mathf.Abs(nextPos4.y)<(height-4)*slotSize/2)){
                    RaycastHit2D ray3 = Physics2D.Raycast( nextPos3, horizontal?Vector2.left:Vector2.down, 0.1f, lyrGround);
                    RaycastHit2D ray4 = Physics2D.Raycast( nextPos4, horizontal?Vector2.left:Vector2.down, 0.1f, lyrGround);
                    if(ray3.collider == null && ray4.collider == null){
                        canExpand = true;
                        invert = true;
                    }
                }
            }
            if(canExpand){
                Vector3 intestine1 = allIntestines[randomIntestine1] + new Vector3(horizontal?invert?-slotSize:slotSize:0,!horizontal?invert?-slotSize:slotSize:0,0);
                Vector3 intestine2 =  allIntestines[randomIntestine2] + new Vector3(horizontal?invert?-slotSize:slotSize:0,!horizontal?invert?-slotSize:slotSize:0,0);
                Debug.DrawLine(allIntestines[randomIntestine1] + new Vector3(horizontal?invert?-slotSize:slotSize:0,!horizontal?invert?-slotSize:slotSize:0,0),allIntestines[randomIntestine1] + new Vector3(horizontal?invert?-slotSize:slotSize:0,!horizontal?invert?-slotSize:slotSize:0,0)+new Vector3(0.1f, 0.1f,0),Color.red);
                Debug.DrawLine(allIntestines[randomIntestine2] + new Vector3(horizontal?invert?-slotSize:slotSize:0,!horizontal?invert?-slotSize:slotSize:0,0),allIntestines[randomIntestine2] + new Vector3(horizontal?invert?-slotSize:slotSize:0,!horizontal?invert?-slotSize:slotSize:0,0)+new Vector3(0.1f, 0.1f,0),Color.green);
                // add those new intestine pieces into the correct point of the array
                allIntestines.Insert(randomIntestine1,intestine1);
                allIntestines.Insert(randomIntestine1,intestine2);
                spawnNumber +=2;
                if(spawnNumber >= length){
                    doneSpawning = true;
                }
            }
        }
        InitializeIntestines(allIntestines, IntestineCorners, IntestineStraight);
    }

    private bool EqualsV3(List<Vector3> V3List, Vector3 V3, float margin = 0.005f)
    {
        for(int i=0; i< V3List.Count-1;i++){
            if(Mathf.Abs(V3List[i].x - V3.x) < margin && Mathf.Abs(V3List[i].y - V3.y) < margin){
                return true;
            }
        }
        return false;
    }
    private void InitializeIntestines(List<Vector3> intestines, GameObject[] IntestineCorners, GameObject[] IntestineStraight){
        for(int i =0; i<intestines.Count;i++){
            float margin = 0.01f;
            int iMin = i-1;
            int iMax = i+1;
            if(iMin<0){
                iMin = intestines.Count-1;
            }
            if(iMax>intestines.Count-1)
            {
                iMax=0;
            }
            Debug.DrawLine(intestines[iMin], intestines[i]);
            if(intestines[iMin].x == intestines[iMax].x){
                //straight horizontally
                Instantiate(IntestineStraight[Random.Range(0,IntestineStraight.Length)],intestines[i],Quaternion.Euler(0,0,intestines[iMin].y > intestines[iMax].y?0:180));
            }else if(intestines[iMin].y == intestines[iMax].y){
                //straight vertically
                Instantiate(IntestineStraight[Random.Range(0,IntestineStraight.Length)],intestines[i],Quaternion.Euler(0,0,intestines[iMin].x > intestines[iMax].x?270:90));
            }else if(intestines[iMin].y > intestines[iMax].y){
                if(intestines[iMin].x > intestines[iMax].x){
                    //curve right to down || up to left
                    if(Mathf.Abs(intestines[i].x - intestines[iMax].x) < margin){
                        //aligned with x2
                        Instantiate(IntestineCorners[Random.Range(0,IntestineCorners.Length)],intestines[i],Quaternion.Euler(0,180,90));
                    }else{
                        //aligned with x1
                        Instantiate(IntestineCorners[Random.Range(0,IntestineCorners.Length)],intestines[i],Quaternion.Euler(0,0,0));
                    }
                }else{
                    //curve left to down || up to right
                    if(Mathf.Abs(intestines[i].x - intestines[iMax].x) < margin){
                        Instantiate(IntestineCorners[Random.Range(0,IntestineCorners.Length)],intestines[i],Quaternion.Euler(0,0,90));
                    }else{
                        Instantiate(IntestineCorners[Random.Range(0,IntestineCorners.Length)],intestines[i],Quaternion.Euler(0,180,0));
                    }
                }
            } else {
                if(intestines[iMin].x > intestines[iMax].x){
                    //curve right to up || down to left
                    if(Mathf.Abs(intestines[i].x - intestines[iMax].x) < margin){
                        Instantiate(IntestineCorners[Random.Range(0,IntestineCorners.Length)],intestines[i],Quaternion.Euler(0,0,270));
                    }else{
                        Instantiate(IntestineCorners[Random.Range(0,IntestineCorners.Length)],intestines[i],Quaternion.Euler(0,180,180));
                    }
                }else{
                    //curve left to up  || down to right
                    if(Mathf.Abs(intestines[i].x - intestines[iMax].x) < margin){
                        Instantiate(IntestineCorners[Random.Range(0,IntestineCorners.Length)],intestines[i],Quaternion.Euler(180,0,90));
                    }else{
                        Instantiate(IntestineCorners[Random.Range(0,IntestineCorners.Length)],intestines[i],Quaternion.Euler(0,0,180));
                    }
                }
            }
        }
    }

    private void FillRemainingTerrain(){
        for(int i=0; i<filledTerrainSlots.Length;i++){
            if(filledTerrainSlots[i]==false){
                Vector2 pos = GetPositionFromArrayUnit(i) + new Vector2(slotSize/2,slotSize/2);
                Debug.DrawLine(pos, pos+new Vector2(0.1f,0.1f));
                RaycastHit2D hit = Physics2D.Raycast(pos, new Vector2(1,1), 0.01f, lyrGround);
                if(hit.collider == null){
                    Instantiate(terrainFiller[Random.Range(0,terrainFiller.Length)],pos,Quaternion.Euler(0,0,Random90DegRotation()));
                }
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
