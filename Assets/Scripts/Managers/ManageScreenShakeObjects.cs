using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageScreenShakeObjects : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 shakeDirection;
    [SerializeField] private float shakeDistance=0.075f;
    private float shakeDuration, shakeTimer;
    private bool isShaking;
    [SerializeField] private float shakeLapses=1;

    private void Start() {
        startPos = transform.position;
    }
    private void Update() {
        if(isShaking){
            shakeTimer+=Time.deltaTime;
            if(shakeTimer>shakeDuration){
                shakeTimer =0;
                isShaking=false;
                transform.position = startPos;
            }else{
                transform.position = startPos + (shakeDirection*shakeDistance* Mathf.Sin((shakeTimer*shakeLapses)/shakeDuration*Mathf.PI));
            }
        }
    }
    public void ShakeAll(float movementTime, Vector2 moveDirection){
        isShaking = true;
        shakeDuration = movementTime;
        shakeDirection = moveDirection;
    }
}
