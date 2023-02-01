using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeScreenShake : MonoBehaviour
{
    private Vector2 startPos;
    private float shakeDepth;
    private Transform ScreenShakeFocus;
    private Vector2 OldScreenShakePos;
    private void Start() {
        startPos = transform.position;
        shakeDepth = GetComponent<SpriteRenderer>().sortingOrder;
        ScreenShakeFocus = GameObject.Find("ScreenShakeManager").transform;
        OldScreenShakePos = ScreenShakeFocus.position;
    }

    private void Update() {
        if(OldScreenShakePos != (Vector2) ScreenShakeFocus.position)
        {
            OldScreenShakePos = (Vector2) ScreenShakeFocus.position;
            transform.position = startPos - ((Vector2)ScreenShakeFocus.position*shakeDepth);
        }
    }
}
