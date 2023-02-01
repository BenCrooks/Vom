using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageScreenShakeObjects : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 shakeDirection;
    private float shakeDistance=0.075f;
    private float shakeDuration, shakeTimer;
    private bool isShaking;

    private void Start() {
        startPos = transform.position;
    }
    private void Update() {
        if(isShaking){
            if(shakeTimer>shakeDuration){
                shakeTimer =0;
                isShaking=false;
                transform.position = startPos;
            }else{
                shakeTimer+=Time.deltaTime;
                transform.position = startPos + (shakeDirection*shakeDistance* Mathf.Sin(shakeTimer/shakeDuration*Mathf.PI));
            }
        }
    }
    public void ShakeAll(float movementTime, Vector2 moveDirection){
        isShaking = true;
        shakeDuration = movementTime;
        shakeDirection = moveDirection;
    }
}
