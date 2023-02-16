using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomizeChildrenSprites : MonoBehaviour
{
    [SerializeField] private Sprite[] OptionalSprites;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<transform.childCount;i++)
        {
            int chosenSprite = Random.Range(0,OptionalSprites.Length);
            for(int j=0; j<i; j++)
            {
                if(OptionalSprites[chosenSprite] == transform.GetChild(j).GetComponent<SpriteRenderer>().sprite){
                    chosenSprite = Random.Range(0,OptionalSprites.Length);
                }
            }
            transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = OptionalSprites[chosenSprite];
        }
    }
}
