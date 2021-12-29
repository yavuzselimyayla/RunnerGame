using UnityEngine;

//How each level ends, when the user hits the end barrier. Loads the next scene
public class EndTrigger : MonoBehaviour {

    //gamemanager instance
    protected GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the player hit the end position, move to next level and play animation
        if(other.tag == "Player")
        {
            //Complete the level
            gameManager.CompleteLevel();
          
            //When finished play the next level animation
            CompleteLevelUiScreen levelUiScreen = FindObjectOfType<CompleteLevelUiScreen>();
            levelUiScreen.playNextLevelAnimation();

        }

       
    }
}
