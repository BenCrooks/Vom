using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    [SerializeField] public Texture2D texture;
    private float size =0.025f/2;
    private float randomOffsetFloat = 0.2f;
    private void Start() {
        Vector3 spawnPos = new Vector3(0,0,0);
        for(int i = 0; i < texture.width; i++) {
            if(i%2==0)
            for(int j=0; j < texture.height; j++){
                if(j%2==0){
                    if(texture.isReadable)
                    {
                        Color col = texture.GetPixel(i,j);
                        if(col.a!=0){
                            spawnPos =  new Vector3(i*size, j*size, 0);
                            Vector3 randomOffset = new Vector3(Random.Range(-randomOffsetFloat,randomOffsetFloat),Random.Range(-randomOffsetFloat,randomOffsetFloat),0);
                            GetComponent<ParticleSystem>().Emit(spawnPos,new Vector3(Random.Range(-2f,2f),0.1f,0) + randomOffset,size*4,Random.Range(0.5f,1.5f),col);
                        }
                    }else{
                        print("make texture readable");
                    }
                }
            }
        }
    }
}
