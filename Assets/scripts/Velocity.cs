using UnityEngine;
using UnityEngine.UI; 

//Update the player velocity so users can see how fast they're going 
public class Velocity : MonoBehaviour {

    //player
    public Transform player;

    protected Rigidbody rb;

    protected GameManager gameManager;

    void Start()
    {
        //setup rigid body connection
        rb = player.GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update () {

        //update the UI 
        updateUiWithSpeed(calculatePlayerSpeed());
    }

    //Calculate the speed from the velocity
    protected int calculatePlayerSpeed()
    {
        return (int)rb.velocity.magnitude;
    }

    //tell gamemanager to update player velocity
    protected void updateUiWithSpeed(int speed)
    {
        gameManager.UpdateVelocity(speed);
    }
}
