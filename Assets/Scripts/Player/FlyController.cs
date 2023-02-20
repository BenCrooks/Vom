using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
public class FlyController : MonoBehaviour
{
    private Rigidbody2D _Rb; 
    private float _Speed = 2000f;
    public GameObject Fly;
    
    private void Start()
    {
        _Rb = gameObject.GetComponent<Rigidbody2D>();
      
    }
    void FixedUpdate()
    {
        if (this.transform.eulerAngles.z > 5f)
        {
            
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (this.transform.eulerAngles.z > -5f)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        //Debug.Log(this.transform.eulerAngles.z);

    }
    public void Movement(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValue<Vector2>());

        _Rb.AddForce(context.ReadValue<Vector2>() * _Speed * Time.deltaTime);
       
    }
    
}
