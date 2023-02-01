using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    [SerializeField] private GameObject mirrorTop, mirrorBottom, mirrorLeft, mirrorRight;
    private Camera cam;
    private Vector2 ScreenScale;
    [SerializeField] private bool stopFallMomentum;
    private Vector3 camStartPos;
    private float boxExtent,xMin, xMax, yMin, yMax;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] terrainObj = GameObject.FindGameObjectsWithTag("Terrain");
        boxExtent = terrainObj[0].GetComponent<BoxCollider2D>().bounds.extents.y;
        foreach(GameObject ter in terrainObj)
        {
            if(ter.transform.position.x - boxExtent < xMin){
                xMin = ter.transform.position.x- boxExtent;
            }
            if(ter.transform.position.x + boxExtent > xMax)
            {
                xMax = ter.transform.position.x+ boxExtent;
            }
            if(ter.transform.position.y - boxExtent < yMin)
            {
                yMin = ter.transform.position.y- boxExtent;
            }
            if(ter.transform.position.y + boxExtent > yMax)
            {
                yMax = ter.transform.position.y+ boxExtent;
            }
        }

        cam = Camera.main;
        // Vector2 lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        // Vector2 upperRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        // ScreenScale = upperRight - lowerLeft;
        ScreenScale = new Vector2(Mathf.Abs(xMin) + Mathf.Abs(xMax), Mathf.Abs(yMin) + Mathf.Abs(yMax));
        mirrorTop.transform.position = transform.position - new Vector3(0,ScreenScale.y,0);
        mirrorBottom.transform.position = transform.position - new Vector3(0,-ScreenScale.y,0);
        mirrorLeft.transform.position = transform.position - new Vector3(ScreenScale.x,0,0);
        mirrorRight.transform.position = transform.position - new Vector3(-ScreenScale.x,0,0);
        camStartPos=cam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < camStartPos.x - ScreenScale.x/2){
            if(GetComponent<CharacterController>().enabled==true){
                transform.position = transform.position + new Vector3(ScreenScale.x,0,0);
            }else{
                transform.position = FindClosestBlock(transform.position + new Vector3(ScreenScale.x,0,0));
                RayAndDestroyBlocks();
            }
        }else
        if(transform.position.x > camStartPos.x+ScreenScale.x/2)
        {
            if(GetComponent<CharacterController>().enabled==true){
                transform.position = transform.position - new Vector3(ScreenScale.x,0,0);
            }else{
                transform.position = FindClosestBlock(transform.position - new Vector3(ScreenScale.x,0,0));
                RayAndDestroyBlocks();
            }
        }
        if(transform.position.y < camStartPos.y-ScreenScale.y/2){
            if(GetComponent<CharacterController>().enabled==true){
                transform.position = transform.position + new Vector3(0,ScreenScale.y,0);
                if(stopFallMomentum){
                    if(!GetComponent<CharacterController>().onGround){
                        gameObject.GetComponent<Rigidbody2D>().velocity -= new Vector2 (0,gameObject.GetComponent<Rigidbody2D>().velocity.y*0.9f);
                    }
                }
            }else{
                transform.position = FindClosestBlock(transform.position + new Vector3(0,ScreenScale.y,0));
                RayAndDestroyBlocks();
            }
        }else
        if(transform.position.y > camStartPos.y+ScreenScale.y/2)
        {

            if(GetComponent<CharacterController>().enabled==true){
                transform.position = transform.position - new Vector3(0,ScreenScale.y,0);
            }else{
                transform.position = FindClosestBlock(transform.position - new Vector3(0,ScreenScale.y,0));
                RayAndDestroyBlocks();
            }
        }
    }

    public Vector3 FindClosestBlock(Vector3 pos)
    {
        float be = boxExtent/2;
        return new Vector3(Mathf.Round(pos.x/be) * be, Mathf.Round(pos.y/be)* be,transform.position.z);
    }
    private void RayAndDestroyBlocks(){
        Vector2 direction = transform.GetChild(0).up;
        Vector3 angledStartPos = new Vector3(boxExtent/2*Mathf.Abs(direction.y), boxExtent/2*Mathf.Abs(direction.x), 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position + angledStartPos, direction, 0.1f,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position - angledStartPos, direction, 0.1f,1 <<LayerMask.NameToLayer("Terrain"));
        if(hit.collider!=null){
            Destroy(hit.collider.gameObject);
        }
        if(hit2.collider!=null){
            Destroy(hit2.collider.gameObject);
        }
    }

}
