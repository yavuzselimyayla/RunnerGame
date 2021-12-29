using UnityEngine;

public class CompleteLevelUiScreen : MonoBehaviour {

    protected GameManager gameManager;

    protected Animator animator; 

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = gameObject.GetComponent<Animator>();
    }

    //Play the next level animation to show the UI 
    public void playNextLevelAnimation()
    {
        Debug.Log("Playing next level animation");
        animator.Play("levelComplete");
    }

    //Called once animation is finished from the UI, load the next level
    public void AnimationFinished()
    {
        Debug.Log("Animation has finished, loading next level");
        gameManager.LoadNextLevel();
    }
    
}
