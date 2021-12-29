using UnityEngine;

/// <summary>
/// Pause menu functionality, handles the toggling of the pause menu, along with the handing of resume, restart and quit
/// </summary>
public class PauseMenu : MonoBehaviour {

    //gamemanger script
    protected GameManager gameManager;

    //Reference to the mobile controls UI
    private MobileControls mobileControlsUI; 

    //The menu
    [Tooltip("Select the child PauseMenu item and drag it here")]
    public GameObject pauseMenuUi;

    //Is paused or not
    public static bool GameIsPaused = false;

   
	// Use this for initialization
	void Start () {
        gameManager = FindObjectOfType<GameManager>();
        mobileControlsUI = FindObjectOfType<MobileControls>();

    }

    private void Update()
    {
        //detect if the player has paused
        DetectPlayerPaused();

    }

    //Detect if the player has opened the pause menu
    void DetectPlayerPaused()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          
            //if already in pause menu, exit pause
            if (GameIsPaused)
            {
                ClosePauseMenu();
                gameManager.mobileControls.EnableMobileControlsUi();
            }
            //not in pause menu, pause it
            else
            {
                StartPauseMenu();
                gameManager.mobileControls.DisableMobileControlsUi();
            }
        }
    }

    //Start the pause menu
    public void StartPauseMenu()
    {
        GameIsPaused = true;
        pauseMenuUi.SetActive(true);
        Time.timeScale = 0f;
    }

    //close the pause menu
    public void ClosePauseMenu()
    {
        GameIsPaused = false;
        pauseMenuUi.SetActive(false);
        Time.timeScale = 1f;
    }

    //quit the game
    public void QuitGame()
    {
        ClosePauseMenu();
        gameManager.QuitGame();
    }

    //restart the current scene
    public void RestartScene()
    {
        ClosePauseMenu();
        gameManager.RestartGame();
    }

}
