using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    [SerializeField] 
    private float damage, rateOfFire, projectileSpeed;
    private float rateOfFireTimer;
    [SerializeField] 
    private LineRenderer lnRndr;
    [SerializeField] private GameObject bullet;
    public bool CanShoot= true;
    [SerializeField] private LayerMask layerAim;
    private Vector3 lastMousePos;
    private Vector2 aimAngle;
    private Vector2 tempAim, tempAim2;
    [SerializeField] private bool shootLineStraight=false;
    [SerializeField] private Transform fireOrigin;
    private bool stoppedAiming=false;
    private bool canShoot=false;
    [SerializeField] private float shootDegradeSpeed;
    [SerializeField] public Material BulletColor;
    private float degradeTimer;
    private bool ContainShooting=true;
    private GameObject ScreenShakeManager;
    private void Start() {
        // lastMousePos = Input.mousePosition;
        if(ScreenShakeManager==null){
            ScreenShakeManager = GameObject.Find("ScreenShakeManager");
        }
        if(ScreenShakeManager==null){
            print("CANNOT FIND SCREEN SHAKE MANAGER");
        }
    }

    public void OnAim(InputAction.CallbackContext context){
        stoppedAiming= context.ReadValue<Vector2>().magnitude<=0.2f?true:false;
        tempAim = context.ReadValue<Vector2>();
    }
    public void OnAimLeftStick(InputAction.CallbackContext context){
        tempAim2 = context.ReadValue<Vector2>();
    }
    public void Shoot(InputAction.CallbackContext context){
        if(!ContainShooting){
            if(context.started){
                canShoot=true;
            }
            if(context.canceled){
                canShoot=false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(CanShoot){
            if(stoppedAiming){
                if(tempAim2.magnitude>0.3f){
                    aimAngle = tempAim2.normalized;
                }else
                if(Vector3.Distance(lastMousePos, tempAim)>10){
                    Cursor.visible = true;

                    var screenPoint = tempAim;
                    aimAngle = (Camera.main.ScreenToWorldPoint(screenPoint) - transform.position).normalized;
                }
            }else
            if(tempAim.magnitude>0.3f){
                if(tempAim.magnitude>2f){
                    Cursor.visible = true;
                    var screenPoint = tempAim;
                    aimAngle = (Camera.main.ScreenToWorldPoint(screenPoint) - transform.position).normalized;
                    aimAngle = Mathf.Abs(aimAngle.x)>Mathf.Abs(aimAngle.y)?new Vector2(aimAngle.x * (1/Mathf.Abs(aimAngle.x)),aimAngle.y* (1/Mathf.Abs(aimAngle.x))):new Vector2(aimAngle.x* (1/Mathf.Abs(aimAngle.y)),aimAngle.y* (1/Mathf.Abs(aimAngle.y)));
                }else{
                    aimAngle = tempAim.normalized;
                }
                // Cursor.visible = false;
            }
            lastMousePos = tempAim;
            if(aimAngle!=new Vector2(0,0)){
                if(shootLineStraight){
                    RaycastHit2D hit = Physics2D.Raycast(fireOrigin.position, aimAngle, Mathf.Infinity,layerAim);
                    Vector3[] points = new Vector3[2];
                    if(hit.collider==null){
                        points[1] = aimAngle*1000;
                    }
                    else{
                        points[1] = hit.point - (Vector2) fireOrigin.position;
                    }
                    lnRndr.SetPositions(points);
                }else{
                    Vector3[] points = new Vector3[20];
                    Vector3 lineScale = aimAngle*20f* ((shootDegradeSpeed-degradeTimer)/shootDegradeSpeed);
                    Vector3 pos0 = fireOrigin.position;
                    Vector3 pos1 = fireOrigin.position + lineScale;
                        Vector3 v = pos0 - pos1;
                        Vector3 P3 = new Vector2(-v.y, v.x) / Mathf.Sqrt(v.x*v.x + v.y*v.y) * Vector2.Distance(pos0,pos1)*new Vector2(0,6);
                        Vector3 P4 = new Vector2(-v.y, v.x) / Mathf.Sqrt(v.x*v.x + v.y*v.y) * -Vector2.Distance(pos0,pos1)*new Vector2(0,6);     
                        Vector3 temppos = P3.y>P4.y?P4:P3;       
                    Vector3 pos2 = pos1 + temppos;
                    for(int o = 0; o<points.Length && o<lnRndr.positionCount;o++){
                        points[o] = GetPoint(pos0,pos1,pos2,(float) o/2 / points.Length);
                        if(o != 0){
                            RaycastHit2D hit = Physics2D.Raycast(points[o-1], -(points[o-1]-points[o]).normalized, Vector2.Distance(points[o-1],points[o]),layerAim);
                            if(hit.collider!=null){
                                points[o] = (Vector2) hit.point;
                                for(int p=0;p < lnRndr.positionCount -o;p++){
                                    points[p+o] = (Vector2) hit.point;
                                }
                                o =lnRndr.positionCount;
                            }
                        }
                    }
                    lnRndr.SetPositions(points);
                }
            }

            rateOfFireTimer+=Time.deltaTime;
            if(canShoot){
                if(degradeTimer<=shootDegradeSpeed){
                    degradeTimer+=Time.deltaTime;
                    if(rateOfFireTimer>=rateOfFire){
                        // if(ScreenShakeManager!=null){
                        //     ScreenShakeManager.GetComponent<ManageScreenShakeObjects>().ShakeAll(0.5f,aimAngle);
                        // }
                        rateOfFireTimer=0;
                        GameObject bul = Instantiate(bullet,fireOrigin.position, Quaternion.Euler(aimAngle));
                        bul.GetComponent<PlayerBullet>().DirectionForce = aimAngle*projectileSpeed* ((shootDegradeSpeed-degradeTimer)/shootDegradeSpeed);
                        bul.GetComponent<PlayerBullet>().Shooter = this.gameObject;
                        bul.GetComponent<PlayerBullet>().ParticleCol = BulletColor.GetColor("_Color4out");
                        bul.GetComponent<SpriteRenderer>().material=BulletColor;
                    }
                }
            }else{
                if(degradeTimer>0){
                    degradeTimer-=Time.deltaTime;
                }else{
                    degradeTimer=0;
                }
            }
        }
    }

    public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
	}
    public void stopShooting(){
        ContainShooting=true;
        canShoot=false;
        lnRndr.enabled=false;
    }
    public void beginShooting(){
        ContainShooting=false;
        lnRndr.enabled=true;
    }
}
