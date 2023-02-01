using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBecomeSpawnable : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnables;
    [SerializeField]private float spawnAmountMin,spawnAmountMax;
    [SerializeField] private Sprite Box;
    [SerializeField] private float boxHealth=20;
    [SerializeField] private bool checkOnStart;
    public bool isBox;
    // Start is called before the first frame update
    void Start()
    {
        if(checkOnStart)
        checkSpawnPosAndPop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator waitAndCheckSpawnable(){
        yield return new WaitForSeconds(Random.Range(0f,2f));
        checkSpawnPosAndPop();
    }
    public void checkSpawnPosAndPop(){
        float extents = GetComponent<BoxCollider2D>().bounds.extents.y*1.1f;
        RaycastHit2D hitTop = Physics2D.Raycast(transform.position + new Vector3(0,extents,0), Vector2.up, extents,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + new Vector3(-extents,0,0), Vector2.left, extents,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + new Vector3(extents,0,0), Vector2.right, extents,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hitBottom = Physics2D.Raycast(transform.position - new Vector3(0,extents,0), Vector2.down, extents,1 <<LayerMask.NameToLayer("Terrain"));
        if(hitTop.collider == null){
            if(hitBottom.collider==null && hitLeft.collider==null && hitRight.collider==null){
                BecomeBox();
            }else{
                if(hitLeft.collider==null && hitRight.collider==null){
                    BecomeBox();
                    hitBottom.collider.gameObject.GetComponent<TerrainBecomeSpawnable>().checkSpawnPosAndPop();
                }else{
                    if(GetComponent<LinkObjectDestruction>()==null){
                        gameObject.tag = "Spawnable";
                    }
                    GetComponent<SpriteRenderer>().color=Color.grey;
                    int GoodiesToSpawn = (int) Random.Range(spawnAmountMin,spawnAmountMax);
                    for(int i =0;i<GoodiesToSpawn;i++){
                        int Goodie = (int) (Mathf.Abs(Random.Range(0-(spawnables.Length-1)/2,(spawnables.Length-1)/2)+Random.Range(-(spawnables.Length-1)/2,(spawnables.Length-1)/2)));//(Random.Range(0,(spawnables.Length)/2)+Random.Range(0,(spawnables.Length)/3)+Random.Range(0,(spawnables.Length)/3));//(Mathf.Cos(Random.Range(0,(spawnables.Length))*Mathf.PI/spawnables.Length)*spawnables.Length);
                        Goodie = Goodie>=spawnables.Length?spawnables.Length:Goodie;
                        GameObject plant = Instantiate(spawnables[Goodie],transform.GetChild(0).transform.position+new Vector3(Random.Range(-extents+0.05f,extents-0.05f),0,0),Quaternion.identity,this.gameObject.transform);
                        plant.transform.localScale = new Vector3(Random.Range(0f,1f)>0.5f?-1:1,1,1);
                    }
                }
            }
        }else{
            if(hitTop.collider.gameObject.GetComponent<TerrainBecomeSpawnable>()!=null){
                if(hitTop.collider.gameObject.GetComponent<TerrainBecomeSpawnable>().isBox){
                    if(hitLeft.collider==null && hitRight.collider==null){
                        if(hitBottom.collider!=null){
                            BecomeBox();
                            hitBottom.collider.gameObject.GetComponent<TerrainBecomeSpawnable>().checkSpawnPosAndPop();
                        }
                    }
                }
            }
        }
    }
    private void BecomeBox(){
        GetComponent<SpriteRenderer>().sprite = Box;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<HealthObj>().Health = boxHealth;
        GetComponent<BoxCollider2D>().size = new Vector2(0.32f,0.32f);
        if(transform.childCount>1){
            foreach (Transform child in transform) {
                if(transform.name!="SpawnPos")
                Destroy(child.gameObject);
            }
        }
        isBox=true;
    }
}
