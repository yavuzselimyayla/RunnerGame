using UnityEngine;
using UnityEngine.UI;
using System; 

//Updated the SCORE text at the top of the screen to show the distance travelled 
public class Score : MonoBehaviour {

    public Transform player;

    //Main scoreboard
    public Text scoreText; 
	
	// Update is called once per frame
	void Update () {
      
       int playerZPos = (int) player.position.z;
       scoreText.text = playerZPos.ToString();
	}

    /// <summary>
    /// Update the players current score by a nominated amount
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateScore(int amount)
    {
        int newScore = Int32.Parse(scoreText.text);
        scoreText.text = newScore.ToString();
    }
}
