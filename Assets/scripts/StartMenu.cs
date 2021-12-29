using UnityEngine;
using UnityEngine.UI;

//handles interactivity for the main menu
public class StartMenu : MonoBehaviour {

    private GameManager gameManager;

    //high score UI component
    private Text highScoreUi; 

	void Start () {
        gameManager = FindObjectOfType<GameManager>();
        highScoreUi = transform.Find("HighScore").GetComponent<Text>();
        LoadHighScore(); 
    }
	
    //On start, start a new game via the manager
	public void OnGameStartButton()
    {
        gameManager.StartNewGame();
    }

    /// <summary>
    /// On quick button, call game manager to quit game
    /// </summary>
    public void OnGameQuitButton()
    {
        gameManager.QuitGame();
    }

    /// <summary>
    /// Load the how to play scene
    /// </summary>
    public void OnLoadHowToPlayButton()
    {
        gameManager.LoadHowToPlayScene();
    }

    /// <summary>
    /// 
    /// On start, load the high score from player preferences to show the user
    /// the highest score attained
    /// </summary>
    protected void LoadHighScore()
    {
        int highScorePref = PlayerPrefs.GetInt("highScore");
        highScoreUi.text = "High Score: " + highScorePref.ToString();
    }
}
