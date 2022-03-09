//
//GameUI Script
//This script controls the UI screens in the game. 
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public GameObject gameHUD;                          //HUD UI
    public GameObject mainMenu;                         //Main Menu UI 
    public GameObject endGame;                          //End Game Screen UI
    public float fadeSpeed = 2.0f;                      //speed for fades in/out
    public float fadeEndSpeed = 1.0f;                   //speed to fade in endgame menu
    public AudioClip StartGameClip;
    public AudioClip ReStartGameClip;

    private GameObject gameWorld;                       //GameWorld object 
    private GameWorldData gameWorldDataScript;          //GameWorld data script

    //----------------------------------------------------------------------------------
    // Start () Use this for initialization
    void Start () {
        gameWorld = GameObject.Find("GameWorld");
        gameWorldDataScript = gameWorld.GetComponent<GameWorldData>();
     
        if(mainMenu)
            mainMenu.SetActive(true);
        if(gameHUD)
            gameHUD.SetActive(true);
        if (endGame)
            endGame.SetActive(false);
    }
    //----------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {
	
	}

    //----------------------------------------------------------------------------------
    //OnStartGame()  called when play game button is pressed
    public void OnStartGame()
    {
        SoundManager.instance.PlayUISingle(StartGameClip);
        StartCoroutine( FadeOutMain()); //fades out into the game
    }
    //start the game
    void HandleStartGame()
    {
        gameWorldDataScript.StartGame();
        if (mainMenu)
            mainMenu.SetActive(false);
        if (gameHUD)
            gameHUD.SetActive(true);
        if (endGame)
            endGame.SetActive(false);
    }
    //----------------------------------------------------------------------------------
    //OnEndGame() called when game is over
    public void OnEndGame()
    {
        if (endGame)
        {
            CanvasGroup canvasGroupEND = endGame.GetComponent<CanvasGroup>();
            endGame.SetActive(true);
            if (canvasGroupEND)
                canvasGroupEND.alpha = 0f; //need to set to 0 so we can fade in 
            StartCoroutine(FadeInEndGame()); //fades endgame menu in, hud out
        }
    }
    //turn on end game menu
    void HandleEndGame()
    {
        if (mainMenu)
            mainMenu.SetActive(false);
        if (gameHUD)
            gameHUD.SetActive(false);
        if (endGame)
            endGame.SetActive(true);
    }
    
    //----------------------------------------------------------------------------------
    //OnRestartGame() called when player chooses to play again (after win or loss)
    public void OnRestartGame()
    {
        if (mainMenu)
        {
            mainMenu.SetActive(true);
            CanvasGroup canvasGroup = mainMenu.GetComponent<CanvasGroup>();
            if(canvasGroup)
            {
                canvasGroup.alpha = 1f;
            }
        }
        if (gameHUD)
        {
            gameHUD.SetActive(true);
            CanvasGroup canvasGroup = gameHUD.GetComponent<CanvasGroup>();
            if (canvasGroup)
            {
                canvasGroup.alpha = 0;
            }

        }
        
        StartCoroutine(FadeOutEnd());
        gameWorldDataScript.RestartGame();

        SoundManager.instance.PlayUISingle(ReStartGameClip);

    }

    //----------------------------------------------------------------------------------
    //OnExit() called when player chooses to exit the game (after win or loss)
    public void OnExit()
    {
        Application.Quit();
    }
    //Fades out main menu, and fades in HUD
    private IEnumerator FadeOutMain()
    {
        if (mainMenu)
        {
            //Fade out Main Menu
            CanvasGroup canvasGroup = mainMenu.GetComponent<CanvasGroup>();
            while (canvasGroup && canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
        //Fade in HUD
        if (gameHUD)
        {
            CanvasGroup canvasGroupHUD = gameHUD.GetComponent<CanvasGroup>();
            while (canvasGroupHUD && canvasGroupHUD.alpha < 1.0f)
            {
                canvasGroupHUD.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
        HandleStartGame();
    }

    //Fades in main menu (for restart), prepares hud for fade in
    private IEnumerator FadeInMain()
    {
        if (mainMenu)
        {
            //Fade in Main Menu
            CanvasGroup canvasGroup = mainMenu.GetComponent<CanvasGroup>();
            while (canvasGroup && canvasGroup.alpha < 1.0f)
            {
                canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
        //set hud to 0
        if (gameHUD)
        {
            CanvasGroup canvasGroupHUD = gameHUD.GetComponent<CanvasGroup>();
            if(canvasGroupHUD)
                canvasGroupHUD.alpha = 0.0f;
        }
    }

    //Fades in the endgame menu after fading out hud
    private IEnumerator FadeInEndGame()
    {
        //Fade out HUD
        if (gameHUD)
        {
            CanvasGroup canvasGroupHUD = gameHUD.GetComponent<CanvasGroup>();
            while (canvasGroupHUD && canvasGroupHUD.alpha > 0)
            {
                canvasGroupHUD.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
        //Fade in End
        if (endGame)
        {
            CanvasGroup canvasGroupEND = endGame.GetComponent<CanvasGroup>();
            while (canvasGroupEND && canvasGroupEND.alpha < 1.0f)
            {
                canvasGroupEND.alpha += Time.deltaTime * fadeEndSpeed;
                yield return null;
            }
        }
        HandleEndGame();
    }

    //Fades out main menu, and fades in HUD
    private IEnumerator FadeOutEnd()
    {
        if (endGame)
        {
            //Fade out Main Menu
            CanvasGroup canvasGroup = endGame.GetComponent<CanvasGroup>();
            while (canvasGroup && canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
        if (endGame)
            endGame.SetActive(false);
    }

}
