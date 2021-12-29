using UnityEngine;

/// <summary>
/// Handles interactivity on the how to play screen menu
/// </summary>
public class HowToPlayScreen : MonoBehaviour {

    private GameManager gameManager;

	// Use this for initialization
	void Start () {
        gameManager = FindObjectOfType<GameManager>();
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
}
