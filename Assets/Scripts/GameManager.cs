using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class GameManager : MonoBehaviour {

    public Maze mazePrefab; //prefab of a maze

    private Maze mazeInstance; //instance of maze

    public PauseMenu pauseMenuPrefab;

    private PauseMenu pauseMenuInstance;

    private float fadeDuration = 1f;

    private Time timeOfFade;

    public static bool paused = false;

    public static bool restart = false;

    public static bool menuPressed = false;

    private bool restarting = false;

    //private Hand hand;

    Camera m_MainCamera;

    GameObject player;

    private static readonly float MENU_DISTANCE = 3f;

    // Use this for initialization
    void Start()
    {
        m_MainCamera = Camera.main;
        //hand = GetComponent<Hand>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if(players.Length > 0)
        {
            player = players[0];
        }
        SteamVR_Events.InputFocus.Listen(OnInputFocus);
        //SteamVR_Fade.Start(Color.black, 0f);
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
         if (menuPressed)
         {
             menuPressed = false;
             paused = !paused;
             if (paused)
             {
                 //Time.timeScale = 0;
                 Vector3 cameraPosition = m_MainCamera.transform.position;
                 pauseMenuInstance = Instantiate(pauseMenuPrefab) as PauseMenu;
                 pauseMenuInstance.transform.position = cameraPosition + (m_MainCamera.transform.forward * MENU_DISTANCE);
                 pauseMenuInstance.transform.rotation = m_MainCamera.transform.rotation;
             }
             if (!paused)
             {
                 Destroy(pauseMenuInstance.gameObject);
                 //Time.timeScale = 1;
             }
         }

         if (restart)
         {
             restarting = true;
             if (pauseMenuInstance != null)
             {
                 Destroy(pauseMenuInstance.gameObject);
             }
             RestartGame();
         }
    }

    private void OnInputFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            if (!paused)
            {
                if (Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }
            }
        }
    }

    //starts a new maze
    private void StartGame()
    {
        mazeInstance = Instantiate(mazePrefab) as Maze; //instantiate our instance of maze from the maze prefab
        mazeInstance.Generate();
    }

    //destroys the old maze and starts a new one
    private void RestartGame()
    {
        Destroy(mazeInstance.gameObject); //destroy old maze
        player.transform.position = Vector3.zero;
        paused = false;
        menuPressed = false;
        restart = false;
        StartGame(); //start new maze
        restarting = false;
    }

}
