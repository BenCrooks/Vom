using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrapBullet : MonoBehaviour
{
    [SerializeField] private GameObject mirrorTop, mirrorBottom, mirrorLeft, mirrorRight;
    private Camera cam;
    private Vector2 ScreenScale;
    [SerializeField] private bool stopFallMomentum;
    private bool passedEdge=false;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Vector2 lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector2 upperRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        ScreenScale = upperRight - lowerLeft;
        mirrorTop.transform.position = transform.position - new Vector3(0,ScreenScale.y,0);
        mirrorBottom.transform.position = transform.position - new Vector3(0,-ScreenScale.y,0);
        mirrorLeft.transform.position = transform.position - new Vector3(ScreenScale.x,0,0);
        mirrorRight.transform.position = transform.position - new Vector3(-ScreenScale.x,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < cam.transform.position.x-ScreenScale.x/2){
            if(passedEdge){
                Destroy(this.gameObject);
            }
            transform.position = transform.position + new Vector3(ScreenScale.x,0,0);
            passedEdge=true;
        }else
        if(transform.position.x > cam.transform.position.x+ScreenScale.x/2)
        {
            if(passedEdge){
                Destroy(this.gameObject);
            }
            transform.position = transform.position - new Vector3(ScreenScale.x,0,0);
            passedEdge=true;
        }
        if(transform.position.y < cam.transform.position.y-ScreenScale.y/2){
            if(passedEdge){
                Destroy(this.gameObject);
            }
            transform.position = transform.position + new Vector3(0,ScreenScale.y,0);
            passedEdge=true;
        }else
        if(transform.position.y > cam.transform.position.y+ScreenScale.y/2)
        {
            if(passedEdge){
                Destroy(this.gameObject);
            }
            transform.position = transform.position - new Vector3(0,ScreenScale.y,0);
            passedEdge=true;
        }
    }
}
