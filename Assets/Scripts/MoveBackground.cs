using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    [SerializeField] private Sprite Left, Mid, Right;
    private SpriteRenderer sprRen;
    [SerializeField] private float resetTime, ResetTimeOffset;
    private float resetTimer;
    [SerializeField] private Color colorRange1, colorRange2;
    
    // Start is called before the first frame update
    void Start()
    {
        resetTime += Random.Range(-ResetTimeOffset,ResetTimeOffset);
        sprRen=GetComponent<SpriteRenderer>();
        sprRen.sprite = Mid;
        sprRen.flipX = Random.Range(0f,1f)>0.5f;

        float ClrMulti = Random.Range(0f,1f);
        Color newCol = new Color(Mathf.Lerp(colorRange1.r,colorRange2.r,ClrMulti),Mathf.Lerp(colorRange1.g,colorRange2.g,ClrMulti),Mathf.Lerp(colorRange1.b,colorRange2.b,ClrMulti));
        sprRen.color = newCol;
        // sprRen.sortingOrder = (int)(-ClrMulti*100);
    }

    // Update is called once per frame
    void Update()
    {
        if(resetTimer<=resetTime){
            resetTimer+=Time.deltaTime;
            if(resetTimer>resetTime){
                sprRen.sprite = Mid;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        GameObject colObj = other.gameObject;
        if(colObj.tag == "Player")
        {
            if(colObj.GetComponent<Rigidbody2D>().velocity.x < 0){
                sprRen.sprite = sprRen.flipX?Right:Left;
                resetTimer=0;
            }else{
                sprRen.sprite = sprRen.flipX?Left:Right;
                resetTimer=0;
            }
        }
    }
}
