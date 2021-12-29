using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Manager for most game functionality, loads stuff and ends game
[DisallowMultipleComponent]
public class GameManager : MonoBehaviour {

    //manages and plays audio
    protected AudioManager audioManager;

    [Header("Screens")]
    //gameover screen
    public GameObject gameOverUi;

    //completed level ui
    public GameObject completeLevelUi;

    //pause screen
    public GameObject PauseMenuUi;

    //mobile controls interface
    public MobileControls mobileControls;

    //main ui element
    public GameObject mainUi;
    //current score, part of the mainUi
    protected Text scoreText;
    //current velocity, part of the mainUI
    protected Text velocityText;
    //current score for the user in this scene
    protected static int score = 0;
    //Current highest score across all games
    protected static int highScore = 0;
    //current velocity of player
    protected static int velocity = 0;

   

    [Header("Other Settings")]
    //game ended (fall off the side or colided)
    public bool gameEnded = false;

    //time in S until it restarts (should match GameOver animation)
    public float restartDelay = 10f;

    //if the player is currently in the start or end menu
    protected bool InMenu = false;


    private void Awake()
    {   
        //get the score text, part of the mainUI element
        scoreText = mainUi.transform.Find("Score").gameObject.GetComponent<Text>();
        velocityText = mainUi.transform.Find("VelocityText").gameObject.GetComponent<Text>();
        mobileControls = FindObjectOfType<MobileControls>();
    }

    //On start set up the level text
    private void Start()
    {

        //setup audio manager instance
        audioManager = AudioManager.instance;

        //Play background music
        audioManager.PlayBackgroundMusic();

        //Current level text UI
        updateCurrentLevelText();

        //clear currenty score if on first level or start menu
        ClearScorePersistent();

        //load player score from persistent storage
        LoadScorePersistent();

        //load player high score from persistent storage
        LoadHighScorePref();

        //Completed level text UI
        setupCompletedLevelText();

        //make sure the score UI is correct on start
        SetupScoreUi();

        //make sure velocity UI is correct on start;
        SetupVelocityUi();

    }

    private void Update()
    {
        //every frame of update, update velocity if needed
        SetupVelocityUi();
    }



    //If the player is in the menu, set bool accordingly
    protected void SetPlayerIsInMenu()
    {
        InMenu = IsPlayerInMenu();
    }

    /// <summary>
    /// Determine if the player is in a menu. Returns true if they are
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerInMenu()
    {
        bool playerInMenu = false;
        int currentSceneId = GetCurrentLevel();
        Scene currentScene = SceneManager.GetSceneByBuildIndex(currentSceneId);
        //true if inside one of the menus
        if(currentScene.name == "start" || currentScene.name == "credits" || currentScene.name == "HowToPlay")
        {
            playerInMenu = true;
        }

        return playerInMenu; 
    }


    //When the player has jumped
    public void PlayJumpSoundEffect()
    {
        audioManager.Play("playerJump");
    }

    //player started moving whoosh sounmd
    public void PlayPlayerMovementStartSoundEffect()
    {
        audioManager.Play("playerStartedMoving");
    }

    //Called when the game has ended (collided with obstacles / fallen off)
    public void EndGame()
    {
        //play death music
        audioManager.Play("playerDeath");
        audioManager.AdjustClipVolume(audioManager.backgroundMusicName, 0.25f);

        //disable mobile control display on death
        mobileControls.DisableMobileControlsUi();

        //play the end game animation, fade to white
        PlayEndGameAnimation();

        if (!gameEnded)
        {
            gameEnded = true;
            Invoke("RestartGame", restartDelay);
        }
    }

    //Restarts the current scene (resertting background music also)
    public void RestartGame()
    {
        audioManager.AdjustClipVolume(audioManager.backgroundMusicName, 1.0f);
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }


    //Completes the current level, showing the UI and adjusting
    //Play end music and adjust the background music
    public void CompleteLevel()
    {
        audioManager.AdjustClipVolume("gameMusic", 0.25f);
        audioManager.Play("playerEndLevel");
        //save current score to persistent storage
        SaveScorePersistent();
        //Disbale mobile controls
        mobileControls.DisableMobileControlsUi();
    }

    //Terminates the game
    public void QuitGame()
    {
        Application.Quit();
    }

    //Called when we want to start the game (moving from menu to game)
    public void StartNewGame()
    {
        //not in menu anymore
        InMenu = false; 
        SceneManager.LoadScene(1);
    }

    //Called via animation event
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(GetCurrentLevel() + 1);
        audioManager.AdjustClipVolume(audioManager.backgroundMusicName, 1.0f);
    }

    public void LoadHowToPlayScene()
    {
     
        SceneManager.LoadScene("HowToPlay"); 
    }

    //sets up the proper text on the completed screen
    //Ensures the level is correct on the completed screen
    private void setupCompletedLevelText()
    {

      
        if(completeLevelUi != null)
        {
            //find the text element child
            if (completeLevelUi.transform.Find("Screen") != null)
            {
                int currentLevel = GetCurrentLevel();
                GameObject completeLevelUiScreen = completeLevelUi.transform.Find("Screen").gameObject;
                GameObject levelTextObject = completeLevelUiScreen.transform.Find("CompleteText").gameObject;
                Text levelText = levelTextObject.GetComponent<Text>();
                levelText.text = "LEVEL " + currentLevel;
            }
        }
       
    }


    //get the current level (scene)
    public int GetCurrentLevel()
    {
        int CurrentLevel = SceneManager.GetActiveScene().buildIndex;
        return CurrentLevel;
    }


    //Update the current level text for the UI
    public void updateCurrentLevelText()
    {
        int currentLevel = GetCurrentLevel();
        string text = "LEVEL " + currentLevel.ToString();
        //Find the level text and change it
        if(mainUi != null)
        {
            GameObject currentLevelTextObject = mainUi.transform.Find("LevelText").gameObject;
            Text currentLevelText = currentLevelTextObject.gameObject.GetComponent<Text>();
            currentLevelText.text = text;
        }

    }


    //On Enable, add another callback to the OnSceneLoaded event
    //Calls our OnSceneLoaded method also
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //When the scene loads, update the current level text
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        //Determine if the player is in the menu
        SetPlayerIsInMenu();

        //If on the final credits screen or main menu, play menu music
        if (IsPlayerInMenu())
        {
            audioManager.Stop(audioManager.backgroundMusicName);
            audioManager.PlayBackgroundMusic();
        }

        //save the high score maybe (if on credit screen)
        SaveHighScorePref();

    }


    //plays the fade in animation and shows the GameOverUi
    public void PlayEndGameAnimation()
    {
        Animator endGameUiAnimator = gameOverUi.GetComponent<Animator>();
        endGameUiAnimator.Play("FadeToWhite");
    }


    /// <summary>
    /// Update the players current score by a nominated amount
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateScore(int amount)
    {
        int newScore = (score + amount);
        scoreText.text = newScore.ToString();
        score = newScore;
    }

    /// <summary>
    /// Called on level start to update the scoreboard
    /// </summary>
    protected void SetupScoreUi()
    {
        scoreText.text = score.ToString();
    }

    /// <summary>
    /// Update the players velocity
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateVelocity(int amount)
    {
        velocity = amount;
    }
    
    /// <summary>
    /// Setup the velocity text for display
    /// </summary>
    protected void SetupVelocityUi()
    {
        velocityText.text = "SPEED " + velocity.ToString();
    }

    /// <summary>
    /// Saves the current score into persistent storage
    /// </summary>
    protected void SaveScorePersistent()
    {
       // Debug.LogError("SAVINGS THE SCORE");
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the player score
    /// </summary>
    protected void LoadScorePersistent()
    {
      //  Debug.LogError("LOADING THE SCORE");
        int scorePref = PlayerPrefs.GetInt("score");
       // Debug.LogError("THE SCORE IS: " + scorePref);
        score = scorePref;
    }

    /// <summary>
    /// Clears the persistent storage for the current score. Used on first load and 
    /// on replay to clear out the current score to start from 0
    /// </summary>
    protected void ClearScorePersistent()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if(currentScene.name == "start" || currentScene.name == "level1")
        {
           // Debug.LogError("CLEARING current score storage");
            PlayerPrefs.SetInt("score", 0);
        }
        
    }

    /// <summary>
    /// Updates the high score saved preference if the current total is greater than
    /// the current saved high score.
    /// </summary>
    protected void SaveHighScorePref()
    {

        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "credits")
        {
            int highScorePref = PlayerPrefs.GetInt("highScore");

            //if current score is greater, save it as new high score
            if (score > highScorePref)
            {
                PlayerPrefs.SetInt("highScore", score);
            }
        }

       
    }

    /// <summary>
    /// Load the high score from persistent storage
    /// </summary>
    protected void LoadHighScorePref()
    {
        highScore = PlayerPrefs.GetInt("highScore"); 
    }
}
