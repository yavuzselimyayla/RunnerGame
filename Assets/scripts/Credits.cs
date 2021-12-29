using UnityEngine;

//On credit screen functionality, restart or quit game
public class Credits : MonoBehaviour {

    //The main gamemanager thing
    public GameManager gameManager; 

    //quit game
    public void QuitGame()
    {
        gameManager.QuitGame();
        
    }

    //Load the first scene, restart
    public void RestartGame()
    {
        gameManager.StartNewGame(); 
    }
	
}
