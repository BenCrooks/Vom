using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagePlayersJoining : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private int numberOfPlayers;
    [SerializeField] private bool addPlayer, removePlayer;



    void Start()
    {
        
    }

    void Update()
    {
        if(addPlayer){
            addPlayer = false;
            AddPlayer();
        }
        if(removePlayer){
            removePlayer=false;
            RemovePlayer();
        }
    }

    private void AddPlayer(){
        numberOfPlayers++;
        anim.SetInteger("Players",numberOfPlayers);
    }
    private void RemovePlayer(){
        numberOfPlayers--;
        anim.SetInteger("Players",numberOfPlayers);
    }
}
