using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public bool wormMode=false;
    public bool StartedGame=false;
    private GameObject[] PlayersReady;
    private GameObject[] SpawnPositions;
    [SerializeField] GameObject UIText;
    [SerializeField] private Material[] charColors, BulletColors;
    [SerializeField] private GameObject player;
    [SerializeField] private float gloatWait, ungloatWait, pauseOnPlayer;
    private float gloatWaitTimer;
    [SerializeField] private float zoomToGloat;
    private float cameraOGZoom;
    private Camera cam;
    private Vector3 camStartPoint;
    private bool gloating;
    private Vector3 lastPlayerPos;
    [SerializeField] private GameObject ScreenShakeManager;
    public GameObject[] spawnLocations;
    [SerializeField] private GameObject DisconnectText;
    [HideInInspector] public int InGamePlayerCount;
    [HideInInspector] public GameObject VictoriousPlayer;
    private bool playerDisconnected=false;
    private Vector3[] disconnectedPlayerPos = new Vector3[4];
    private float[] disconnectedPlayerHealth = new float[4];
    private int[] originalPlayerOrder = {0,1,2,3};
    private bool playTransformSoundOnce=true;
    [SerializeField] private GameObject[] HealthUI;
 
    // Start is called before the first frame update
    void Start()
    {

        cam = Camera.main;
        cameraOGZoom= cam.orthographicSize;
        camStartPoint = cam.transform.position;
        if(ScreenShakeManager==null){
            ScreenShakeManager = GameObject.Find("ScreenShakeManager");
        }
        if(ScreenShakeManager==null){
            print("CANNOT FIND SCREEN SHAKE MANAGER");
        }
        Time.timeScale =1;
        StartCoroutine(delayCheck());
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayersReady!=null){
            if(PlayersReady.Length>1)
            {
                if(StartedGame){
                    if(InGamePlayerCount == 1){
                        if(VictoriousPlayer != null){
                            if(!gloating){
                                gloatWaitTimer+=Time.deltaTime;
                                if(gloatWaitTimer >= gloatWait-pauseOnPlayer){
                                    if(gloatWaitTimer >= gloatWait){
                                        if(gloatWaitTimer >= gloatWait + ungloatWait){
                                            gloatWaitTimer=0;
                                            gloating = true;
                                            cam.orthographicSize = cameraOGZoom;
                                            cam.transform.position = camStartPoint;
                                        }else{
                                            float t = (gloatWaitTimer-gloatWait)/(ungloatWait*Vector2.Distance(new Vector2(0,0),VictoriousPlayer.transform.position)/5f);
                                            cam.orthographicSize = Mathf.Lerp(zoomToGloat,cameraOGZoom, t);
                                            cam.transform.position = new Vector3( Mathf.Lerp(lastPlayerPos.x,camStartPoint.x, t),Mathf.Lerp(lastPlayerPos.y,camStartPoint.y, t),camStartPoint.z);
                                        }
                                    }else{
                                        lastPlayerPos = VictoriousPlayer.transform.position;
                                        float t2 = Mathf.Lerp(1f,2f,(gloatWaitTimer-(gloatWait-pauseOnPlayer))/gloatWait);
                                        VictoriousPlayer.transform.localScale = new Vector3(t2,t2,t2);
                                    }
                                    if(gloatWaitTimer>4.8f){
                                        if(playTransformSoundOnce){
                                            GetComponent<AudioSource>().Play();
                                            playTransformSoundOnce=false;
                                        }
                                    }
                                    VictoriousPlayer.GetComponent<PlayerHealth>().immune=true;
                                    SetWormMode();
                                }else{
                                    cam.orthographicSize = Mathf.Lerp(cameraOGZoom,zoomToGloat,gloatWaitTimer/(gloatWait-pauseOnPlayer));
                                    cam.transform.position = new Vector3( Mathf.Lerp(camStartPoint.x,VictoriousPlayer.transform.position.x, gloatWaitTimer/(gloatWait-pauseOnPlayer)),Mathf.Lerp(camStartPoint.y,VictoriousPlayer.transform.position.y, gloatWaitTimer/gloatWait),camStartPoint.z);
                                }
                            }
                        }
                    }else{
                        for(int i=0;i< PlayersReady.Length;i++){
                            if(PlayersReady[i] != null){
                                disconnectedPlayerPos[i] = PlayersReady[i].transform.position;
                                disconnectedPlayerHealth[i] = PlayersReady[i].GetComponent<PlayerHealth>().Health;
                            }
                        }
                    }
                }
            }
        }
    }
    IEnumerator delayCheck()
    {
        yield return new WaitForSecondsRealtime(2f);
        InputSystem.onDeviceChange +=
        (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    CheckPlayerCountAndChangeText();
                    break;

                case InputDeviceChange.Removed:
                    RemovePlayer();
                    break;
            }
        };

        StopAllCoroutines();
        StartCoroutine(delayCheck());
    }

    public void SetWormMode(){
        if(!wormMode){
            if(InGamePlayerCount == 1){
                wormMode = true;
                VictoriousPlayer.GetComponent<ControllerManager>().TransformToWorm();
                foreach(GameObject plr in PlayersReady){
                    plr.GetComponent<AimController>().stopShooting();
                }
            }else{
                print("No Worms 4 u");
                wormMode=false;
            }
        }
    }
    public void StopWormMode(){
        if(wormMode){
            VictoriousPlayer=null;
            InGamePlayerCount = PlayersReady.Length;
            gloating=false;
            wormMode=false;
            //SpawnEggs
            GameObject[] ter = GameObject.FindGameObjectsWithTag("Terrain");
            foreach(GameObject terr in ter){
                terr.GetComponent<TerrainBecomeSpawnable>().checkSpawnPosAndPop();
            }
            SpawnPositions = GameObject.FindGameObjectsWithTag("Spawnable");
            for(int i=0; i< PlayersReady.Length; i++){
                PlayersReady[i].SetActive(true);
                PlayersReady[i].GetComponent<AimController>().beginShooting();
                PlayersReady[i].transform.position = SpawnPositions[Random.Range(0,SpawnPositions.Length-1)].transform.GetChild(0).transform.position;
                PlayersReady[i].GetComponent<PlayerHealth>().ResetPlayerHealth();
                PlayersReady[i].GetComponent<CharacterController>().CanMove=true;
                PlayersReady[i].GetComponent<AimController>().CanShoot=true;
            }
            playTransformSoundOnce=true;
        }
    }
    public void CheckPlayerCountAndChangeText(){
        if(!StartedGame){
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if(players.Length>1){
                UIText.GetComponent<Text>().text = "Press Start To Begin";
            }
            for(int i=0; i<players.Length;i++){
                SetPlayerColor(players[i],i);
                if(HealthUI[i]!=null){
                    players[i].GetComponent<PlayerHealth>().HealthUI = HealthUI[i];
                    HealthUI[i].GetComponent<WatchPlayersHealth>().UpdateHealthUI(6);
                }else{
                    print("healthUI unassigned");
                }
                if(i==players.Length-1){
                    players[i].transform.position = spawnLocations[i].transform.position;
                }
            }
        }else{
            if(playerDisconnected){
                Time.timeScale = 1;
                DisconnectText.SetActive(false);
                playerDisconnected=false;
                GetComponent<PlayerInputManager>().joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

                PlayersReady = GameObject.FindGameObjectsWithTag("Player");
                int playerLost=-1;
                int posLost =-1;
                bool[] posFound = new bool[PlayersReady.Length];
                int[] tempPlayerOrder = new int[4];
                for(int i=0; i<originalPlayerOrder.Length;i++){
                    tempPlayerOrder[i] = originalPlayerOrder[i];
                }
                for(int i=0; i<PlayersReady.Length; i++){
                    bool foundPlayer = false;
                    for(int j=0;j<PlayersReady.Length;j++){
                        if(!posFound[j]){
                            if(disconnectedPlayerPos[j]!=null){
                                if(disconnectedPlayerPos[j] != new Vector3(0,0,0)){
                                    if (PlayersReady[i].transform.position == disconnectedPlayerPos[j])
                                    {
                                        foundPlayer = true;
                                        posFound[j] = true;
                                        originalPlayerOrder[i] = tempPlayerOrder[j];
                                    }
                                }
                            }
                        }
                    }
                    if(foundPlayer == false){
                        playerLost=i;
                    }
                }
                for(int i=0; i<posFound.Length; i++){
                    if(posFound[i] !=true)
                    {
                        posLost = i;
                        originalPlayerOrder[playerLost] = tempPlayerOrder[posLost];
                    }
                }
                if(playerLost != -1){
                    PlayersReady[playerLost].GetComponent<PlayerHealth>().Health = disconnectedPlayerHealth[posLost];
                    PlayersReady[playerLost].transform.position = disconnectedPlayerPos[posLost];
                    PlayersReady[playerLost].GetComponent<AimController>().beginShooting();
                    SetPlayerColor(PlayersReady[playerLost], originalPlayerOrder[playerLost]);
                    if(HealthUI[originalPlayerOrder[playerLost]]!=null){
                        PlayersReady[playerLost].GetComponent<PlayerHealth>().HealthUI = HealthUI[originalPlayerOrder[playerLost]];
                        HealthUI[originalPlayerOrder[playerLost]].GetComponent<WatchPlayersHealth>().UpdateHealthUI((int)disconnectedPlayerHealth[posLost]);
                    }else{
                        print("healthUI unassigned");
                    }
                }else{
                    print("could not find disconnected player details");
                }
            }
        }
    }
    public void BeginGame(){
        if(!StartedGame){
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if(players.Length>1){
                StartedGame=true;
                UIText.GetComponent<Text>().text = "";
                PlayersReady = players;
                for(int i=0; i<PlayersReady.Length;i++){
                    PlayersReady[i].GetComponent<AimController>().beginShooting();
                    PlayersReady[i].transform.position = spawnLocations[i].transform.position;
                }
                InGamePlayerCount = PlayersReady.Length;
                GetComponent<PlayerInputManager>().joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
            }
        }
    }
    
    public void RemovePlayer(){
        if(StartedGame){
            playerDisconnected=true;
            Time.timeScale = 0;
            if(DisconnectText!=null){
                DisconnectText.SetActive(true);
            }
            GetComponent<PlayerInputManager>().joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
            print("player left");
        }
    }

    private void SetPlayerColor(GameObject player, int i){
        player.GetComponent<PlayerHealth>().sprites[0].material = charColors[i];
        player.GetComponent<PlayerHealth>().sprites[1].material = charColors[i];
        player.GetComponent<PlayerHealth>().sprites[2].material = charColors[i];
        player.GetComponent<PlayerHealth>().sprites[3].material = charColors[i];
        player.GetComponent<PlayerHealth>().sprites[4].material = charColors[i];
        player.GetComponent<AimController>().BulletColor = BulletColors[i];
    }
}
