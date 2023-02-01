using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WormController : MonoBehaviour
{
    private Vector2 facingDirection = new Vector2(0,1), cameFromDirection= new Vector2(0,-1);
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject WormPiece;
    [SerializeField] private Sprite WormStraight1, WormStraight2, WormCorner1, WormCorner2;
    private float moveTimer;
    private float DistanceToMove = 0.3f;
    [SerializeField] private GameObject[] animators;
    private bool cornering = false, lastUpwards=true, firstWormPiece = true, turningRight;
    private Vector2 lastAngle=new Vector2(0,1);
    [SerializeField] private GameObject wormDetails, Terrain;
    private GameObject[] WormBits = new GameObject[170];
    private int wormBitCount=0, wormBitTotal;
    private bool canMove=true;
    [SerializeField] private AudioClip EatingAmbient, turningSound, wormDeath;
    [SerializeField] private GameObject tempAudio;


    void Start()
    {
        wormBitTotal=WormBits.Length-1;
        GameObject ter = Instantiate(Terrain, new Vector3(100000,0,0),Quaternion.identity);
        Bounds bounds = ter.GetComponent<BoxCollider2D>().bounds;
        Destroy(ter);
        DistanceToMove = bounds.extents.y*2f;
        initiateFirstBlock();
    }

    public void initiateFirstBlock(){
        firstWormPiece=true;
        GameObject first = Instantiate(WormPiece,transform.position - new Vector3(0,0,0), animators[0].transform.rotation);
        first.GetComponent<SpriteRenderer>().material = animators[0].GetComponent<SpriteRenderer>().material;
        WormBits[0] = first;
        RayAndDestroyBlocks();
        wormDetails.transform.localScale = new Vector3(1,1,1);
        wormDetails.transform.rotation = Quaternion.Euler(0,0,0);
        lastAngle =new Vector2(0,1);
        facingDirection = lastAngle;
        lastUpwards=true;
        GetComponent<AudioSource>().volume =1f;
        GetComponent<AudioSource>().clip = EatingAmbient;
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().loop=true;
    }

    public void OnMove(InputAction.CallbackContext context){
        if(GetComponent<ControllerManager>().isWorm){
            Vector2 InputDirection = context.ReadValue<Vector2>();
            if(!firstWormPiece){
                if(Mathf.Abs(InputDirection.x)>0.2f||Mathf.Abs(InputDirection.y)>0.2f){
                    Vector2 GoalDirection = Mathf.Abs(InputDirection.x) > Mathf.Abs(InputDirection.y) ? new Vector2(Mathf.Sign(InputDirection.x),0) : new Vector2(0,Mathf.Sign(InputDirection.y));
                    if(GoalDirection != cameFromDirection){
                        facingDirection = GoalDirection;
                        lastAngle=GoalDirection;
                    }
                }
            }
        }
    }

    void Update()
    {
        moveTimer+=Time.deltaTime;
        if(moveTimer>=moveSpeed){
            moveTimer=0;
            Move();
        }else
        if(moveTimer >= moveSpeed - 0.2f){
            Animate("WormBite",true);
        }else{
            if(moveTimer >= 0.4f){
                Animate("WormBite",false);
            }
        }
    }
    private void Move(){
        if(canMove){
            if(firstWormPiece){
                firstWormPiece=false;
                wormDetails.SetActive(true);
                wormDetails.transform.GetChild(1).GetComponent<SpriteRenderer>().material = animators[0].GetComponent<SpriteRenderer>().material;
            }else{
                if(Mathf.Abs(lastAngle.y)>Mathf.Abs(lastAngle.x)){
                    cornering=!lastUpwards;
                    lastUpwards=true;
                    if(lastAngle.y < 0.01f){
                        RotateSprites(180);
                    }else{
                        RotateSprites(0);
                    }
                }else{
                    cornering=lastUpwards;
                    lastUpwards=false;
                    if(lastAngle.x < 0f){
                        RotateSprites(90);
                        SpriteFlip(true);
                    }else{
                        RotateSprites(-90);
                        SpriteFlip(false);
                    }
                }
                if(cornering){
                    if(tempAudio!=null){
                        GameObject qp = Instantiate(tempAudio,transform.position,Quaternion.identity);
                        qp.GetComponent<PlayAudioAndDelete>().clip = turningSound;
                        qp.GetComponent<AudioSource>().pitch += Random.Range(-0.15f,0.15f);
                    }
                    if(Mathf.Abs(-1*cameFromDirection.y)>Mathf.Abs(-1*cameFromDirection.x)){
                        if(lastAngle.x>0){
                            turningRight = -1*cameFromDirection.y>0;
                        }else{
                            turningRight = -1*cameFromDirection.y<0;
                        }
                    }else{
                        if(lastAngle.y>0){
                            turningRight = -1*cameFromDirection.x<0;
                        }else{
                            turningRight = -1*cameFromDirection.x>0;
                        }
                    }
                }
                GameObject wrmPce = Instantiate(WormPiece,transform.position - new Vector3(0,0,0), animators[0].transform.rotation);
                wrmPce.GetComponent<SpriteRenderer>().material = animators[0].GetComponent<SpriteRenderer>().material;
                wormBitCount++;
                WormBits[wormBitCount] = wrmPce;
                if(wormBitCount == wormBitTotal)
                {
                    DestroyAllWormBits();
                }else{
                    if(cornering){
                        float turnangle = 0;
                        if(Mathf.Abs(lastAngle.x) > Mathf.Abs(lastAngle.y)){
                            if(lastAngle.x<0)
                            {
                                if(turningRight){
                                    turnangle = 270;
                                }else{
                                    turnangle = 0;
                                }
                            }else{
                                if(turningRight){
                                    turnangle = 90;
                                }else{
                                    turnangle = 180;
                                }
                            }
                        }else{
                            //turning vertical
                            if(lastAngle.y<0)
                            {
                                if(turningRight){
                                    turnangle = 0;
                                }else{
                                    turnangle = 90;
                                }
                            }else{
                                if(turningRight){
                                    turnangle = 180;
                                }else{
                                    turnangle = -90;
                                }
                            }                }
                        wrmPce.transform.rotation = Quaternion.Euler(0,0,turnangle);
                        if(Random.Range(0f,1f)<0.5f){
                            wrmPce.GetComponent<SpriteRenderer>().sprite = WormCorner1;
                        }else{
                            wrmPce.GetComponent<SpriteRenderer>().sprite = WormCorner2;
                        }
                    }else{
                        if(Random.Range(0f,1f)<0.5f){
                            wrmPce.GetComponent<SpriteRenderer>().sprite = WormStraight1;
                        }else{
                            wrmPce.GetComponent<SpriteRenderer>().sprite = WormStraight2;
                        }
                    }
                }
            }
            RayAndDestroyBlocks();
            cornering=false;
            // wrmPce
            transform.position += (Vector3) facingDirection*DistanceToMove;
            cameFromDirection = facingDirection*-1;
        }
    }
    private void RayAndDestroyBlocks(){
        Vector3 angledStartPos = DistanceToMove/2 * transform.GetChild(0).right;
        Vector3 StartRayOffset = 0.05f* -transform.GetChild(0).up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + angledStartPos +StartRayOffset, lastAngle, 0.1f,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position - angledStartPos +StartRayOffset, lastAngle, 0.1f,1 <<LayerMask.NameToLayer("Terrain"));
        // Debug.DrawLine(transform.position + angledStartPos + StartRatOffset, transform.position + angledStartPos + (Vector3)(lastAngle*0.1f),Color.yellow,1000f);
        // Debug.DrawLine(transform.position - angledStartPos + StartRatOffset, transform.position - angledStartPos + (Vector3)(lastAngle*0.1f),Color.yellow,1000f);
        // Debug.DrawLine(transform.position + angledStartPos + StartRatOffset+angledStartPos, transform.position + angledStartPos + StartRatOffset-angledStartPos,Color.yellow,1000f);
        // Debug.DrawLine(transform.position - angledStartPos + StartRatOffset+angledStartPos, transform.position - angledStartPos + StartRatOffset-angledStartPos,Color.yellow,1000f);

        if(hit.collider!=null){
            DelObj(hit.collider.gameObject);
        }
        if(hit2.collider!=null){
            DelObj(hit2.collider.gameObject);
        }
        if(cornering){
            RaycastHit2D hit3 = Physics2D.Raycast(transform.position + angledStartPos -StartRayOffset, -lastAngle, 0.1f,1 <<LayerMask.NameToLayer("Terrain"));
            RaycastHit2D hit4 = Physics2D.Raycast(transform.position - angledStartPos-StartRayOffset, -lastAngle, 0.1f,1 <<LayerMask.NameToLayer("Terrain"));
            // Debug.DrawLine(transform.position + angledStartPos-StartRayOffset, transform.position + angledStartPos-StartRayOffset + (Vector3)(-lastAngle*0.1f),Color.red,1000f);
            // Debug.DrawLine(transform.position - angledStartPos-StartRayOffset, transform.position - angledStartPos-StartRayOffset + (Vector3)(-lastAngle*0.1f),Color.red,1000f);
            if(hit3.collider!=null){
                DelObj(hit3.collider.gameObject);
            }
            if(hit4.collider!=null){
                DelObj(hit2.collider.gameObject);
            }
        }
    }

    private void DelObj(GameObject obj){
        if(obj.GetComponent<ExplodeWorm>()!=null){
            obj.GetComponent<ExplodeWorm>().Explode(0);
        }else{
            Destroy(obj);
        }
    }

    private void Animate(string WhatToSet, bool ValueBool){
        for(int i=0; i<animators.Length;i++){
            animators[i].GetComponent<Animator>().SetBool(WhatToSet,ValueBool);
        }
    }

    private void SpriteFlip(bool flip){
        for(int i=0; i<animators.Length;i++){
            animators[i].GetComponent<SpriteRenderer>().flipX = flip;
        }
        wormDetails.transform.localScale = new Vector3(flip?-1:1,1,1);
    }
    private void RotateSprites(float deg){
        for(int i=0; i<animators.Length;i++){
            animators[i].transform.rotation = Quaternion.Euler(0,0,deg);
        }
        wormDetails.transform.rotation = Quaternion.Euler(0,0,deg);
    }
    private void DestroyAllWormBits(){
        GetComponent<AudioSource>().volume =0.7f;
        GetComponent<AudioSource>().loop=false;
        GetComponent<AudioSource>().clip = wormDeath;
        GetComponent<AudioSource>().Play();
        foreach(GameObject wrm in GameObject.FindGameObjectsWithTag("Worm")){
            wrm.GetComponent<ExplodeWorm>().Explode();
        }
        foreach(GameObject ter in GameObject.FindGameObjectsWithTag("Terrain")){
            ter.GetComponent<TerrainBecomeSpawnable>().StartCoroutine("waitAndCheckSpawnable");
        }
        StartCoroutine(WaitBeforePlayerActivate());
        canMove=false;
    }
    private IEnumerator WaitBeforePlayerActivate(){
        yield return new WaitForSeconds(4f);
        GetComponent<AudioSource>().Stop();
        canMove=true;
        wormBitCount=0;
        transform.position = lastAngle*-1*DistanceToMove;
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().StopWormMode();
        this.enabled=false;
        lastAngle=new Vector2(0,1);
        facingDirection= new Vector2(0,1);

        wormDetails.SetActive(false);
        GetComponent<ControllerManager>().TransformToCharacter();
    }
}
