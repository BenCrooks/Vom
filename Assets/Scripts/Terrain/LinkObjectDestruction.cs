using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkObjectDestruction : MonoBehaviour
{
    public GameObject OtherObj;
    private Vector2 ScreenScale;
    [SerializeField] private bool autoFindTerrain = false;
    [SerializeField] private bool checkH, checkV;
    // Start is called before the first frame update
    void Awake()
    {
        if(autoFindTerrain){
            Camera cam = Camera.main;
            Vector2 lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
            Vector2 upperRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            ScreenScale = upperRight - lowerLeft;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(checkH?ScreenScale.x:0, checkV? ScreenScale.y:0, 0), Vector2.right, 0.1f,1 <<LayerMask.NameToLayer("Terrain"));
            if(hit.collider!=null){
                OtherObj = hit.collider.gameObject;
            }else{
                RaycastHit2D hit2 = Physics2D.Raycast(transform.position - new Vector3(checkH?ScreenScale.x:0, checkV? ScreenScale.y:0, 0), Vector2.right, 0.1f,1 <<LayerMask.NameToLayer("Terrain"));
                if(hit2.collider!=null){
                    OtherObj = hit2.collider.gameObject;
                }else{
                    print("couldnt find terrain");
                    GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }
    void Start(){
        if(autoFindTerrain){
            if(OtherObj!=null){
                if(OtherObj.GetComponent<LinkObjectDestruction>()!=null){
                    if(OtherObj.GetComponent<LinkObjectDestruction>().OtherObj==null){
                        OtherObj.GetComponent<LinkObjectDestruction>().OtherObj = this.gameObject;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy() {
        if(OtherObj!=null){
            if(OtherObj.GetComponent<LinkObjectDestruction>()!=null)
            {
                OtherObj.GetComponent<LinkObjectDestruction>().DestroyNextFrame();
            }
        }
    }
    public void DestroyNextFrame(){
        Destroy(this.gameObject,0.01f);
    }
}
