using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    //player movement script
    public PlayerMovement movement;

    //the collision material
    public Material playerCollisionMaterial;

    //game manager instance
    protected GameManager gameManager;

    //manages the audio
    protected AudioManager audioManager; 

 
    private void Start()
    {
        //find the instance of game manager
        gameManager = FindObjectOfType<GameManager>();
        //get singleton for the audio manager
        audioManager = AudioManager.instance;
    }


    //Determine collision on stuff
    private void OnCollisionEnter(Collision collisionInfo)
    {
       

        string objectTag = collisionInfo.collider.tag;
        //stop player movement on collision
        if (objectTag == "obstacle"){
            processCollisionWithObstacle();
        }
        else if (objectTag == "ground"){
            processCollisionWithFloor();
        }
    }

    //player hit obstacle 
    void processCollisionWithObstacle()
    {
        setPlayerCollidedMaterial();
        setPlayerMovementDisabled();

        audioManager.Play("playerHitObstacle");
        gameManager.EndGame();
    }

    //player hit the floor (no longer in the air or falling)
    void processCollisionWithFloor()
    {
        //This was done because as the scene loads it would trigger collision, couldnt find out why
        if(movement.playerCanMove == true)
        {
            audioManager.Play("playerHitGround");
            movement.isFalling = false;
            movement.isInAir = false;
        }
       
    }

    //When a player has crashed, update its material
    protected void setPlayerCollidedMaterial()
    {
        Renderer renderer = transform.GetComponent<Renderer>();
        renderer.material = playerCollisionMaterial;
    }

    //set the player movement to be disabled
    protected void setPlayerMovementDisabled()
    {
        movement.enabled = false;
    }

   


}
