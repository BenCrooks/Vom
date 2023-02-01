using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchPlayersHealth : MonoBehaviour
{    
    public void UpdateHealthUI(int health){
        health-=1;
        for(int i=0;i<health;i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }
        for(int i=4;i>=0;i--){
            if(i>health){
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
