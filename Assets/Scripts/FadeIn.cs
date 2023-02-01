using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public bool fadeIn;
    [SerializeField] private float FadeTime;
    [SerializeField] private Color startColor, endColor;
    private float FadeCounter;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeIn){
            FadeCounter+=Time.deltaTime;
            if(FadeCounter<=FadeTime){
                GetComponent<SpriteRenderer>().color = new Vector4(Mathf.Lerp(startColor.r,endColor.r,FadeCounter/FadeTime),Mathf.Lerp(startColor.g,endColor.g,FadeCounter/FadeTime),Mathf.Lerp(startColor.b,endColor.b,FadeCounter/FadeTime),Mathf.Lerp(startColor.a,endColor.a,FadeCounter/FadeTime));
            }else{
                fadeIn=false;
                GetComponent<SpriteRenderer>().color = endColor;
            }
        }
    }
}
