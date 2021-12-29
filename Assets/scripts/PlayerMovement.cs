using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {


    //current player rigid body
    public Rigidbody rb;

    //reference to the ground object
    public Rigidbody ground;

    //gamemanger thing
    protected GameManager gameManager;

    //player speed settings
    [Space]
    [Header("Player Speed Forces")]
    public float rbForwardForce = 2000f;
    public float rbSidewaysForce = 500f;
    public float rbUpwardsForce = 500f;
    public float rbDownwardsForce = 500f; 

    protected int leftMouseButton = 0;
    protected int rightMouseButton = 1;

    //movement related stuff
    [Space]
    [Header("Player Movement")]
    public bool isMovingLeft = false;
    public bool isMovingRight = false;
    public bool isFalling = false;
    public bool isClickingJump = false;
    public bool isInAir = false;
    public bool IsFallingToDeath = false; //player falling off the edge

    [Range(0.0f, 3.0f)]
    public float playerDropTime = 0.25f;
    //time the player stands still as the sceen loads.
    [Range(0.0f, 5.0f)]
    public float playerMovementDisabledTime = 2.0f;
    public bool playerCanMove = false;
    
    //transform related stuff
    [Space]
    [Header("Player Transform")]
    [Tooltip("Player transformed into twice height half width element")]
    public bool isThin = false;
    [Tooltip("Player transformed into half height twice width element")]
    public bool isFlat = false;
    [Tooltip("Player is using the regular shape")]
    public bool IsRegular = true;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        FreezePlayerRotation();


    }

    /// <summary>
    /// Freeze the rotation for the player so that they cant rotate 
    /// </summary>
    private void FreezePlayerRotation()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    //normal update, used for input detection
    private void Update()
    {
        //detect movement
        DetectPlayerInput();

        //detect if player can move yet
        DetectPlayerStart();
    }

    //Fixed update, used for physics etc
    void FixedUpdate()
    {
        //handle movement for player
        HandlePlayerMovement();

        //ensure player has not fallen out of bounds
        HandlePlayerOutOfBounds();

        //transform the player shape if required
        HandlePlayerShapeTransform();
      
    }

    //Detect how long since this scene was loaded and only move player if it's time to start
    void DetectPlayerStart()
    {
        float TimeSinceLoad = Time.timeSinceLevelLoad;
        playerCanMove = (TimeSinceLoad < playerMovementDisabledTime) ? false : true; 
        
    }


    //Check for input keys, these values to be used in handlePlayerMovement
    void DetectPlayerInput()
    {
        //do nothing if the player hasn't started yet
        if(playerCanMove == false)
        {
            return; 
        }


        //Detect if holding down left or right movement keys
        //moving right
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            isMovingLeft = false;
            if (!isInAir)
            {
                isMovingRight = true;
            }
          
        }
        //moving left
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            isMovingRight = false;
            if (!isInAir)
            {
                isMovingLeft = true;
            }
        }
       

        //released keys (either left or right)
        if(Input.GetKeyUp("d") || Input.GetKeyUp("right"))
        {
            isMovingRight = false;
        }
        if(Input.GetKeyUp("a") || Input.GetKeyUp("left")){
            isMovingLeft = false;
        }



        //jumping (can jump while moving left and right)
        //if (Input.GetMouseButtonDown(leftMouseButton) || Input.GetKey("space"))
        if (Input.GetKey("space"))
        {
            //jump only if we can
            if (PlayerCanJump())
            {
                isClickingJump = true;
            }
        }


        //Handle player transformation (so long as not jumping)
        if (!isInAir)
        {
            //player transforms (to thin or flat)
            if (Input.GetKeyDown("w"))
            {
                isThin = true;
            }
            else if (Input.GetKeyDown("s"))
            {
                isFlat = true;
            }

            //Detect when user has stopped pressing transform buttons
            if (Input.GetKeyUp("w"))
            {
                isThin = false;
            }
            else if (Input.GetKeyUp("s"))
            {
                isFlat = false;
            }
        }


    }

    //Handle player movement based on which buttons the player is presssing and if they're jumping
    void HandlePlayerMovement()
    {

        //do nothing if the player hasn't started yet
        if (playerCanMove == false)
        {
            return;
        }

        //if we're moving but velocity is 0, it's the first frame of movement
        if(rb.velocity.magnitude == 0)
        {
            gameManager.PlayPlayerMovementStartSoundEffect();
        }



        //calculate forward and side forces
        float forwardForce = 0f;
        float sidewaysForce = 0f;
        float verticalForce = 0f;


        ForceMode forceMode = ForceMode.Force;
        Vector3 force = new Vector3(0, 0, 0);

        //Moving right
        if (isMovingRight)
        {
            if (!isInAir)
            { 
                sidewaysForce = setupForce(rbSidewaysForce, 1f);
                forwardForce = setupForce(rbForwardForce, 0.7f);
                verticalForce = 0f;
            }

        }
        //going left
        else if (isMovingLeft)
        {
            if (!isInAir)
            {
                sidewaysForce = -setupForce(rbSidewaysForce, 1f);
                forwardForce = setupForce(rbForwardForce, 0.7f);
                verticalForce = 0f;
            }

        }
        //jumping (only triggered once)
        else if (isClickingJump && !isInAir)
        {

            gameManager.PlayJumpSoundEffect();
            isClickingJump = false;
            isInAir = true;

            sidewaysForce = 0f;
            forceMode = ForceMode.VelocityChange;
            forwardForce = setupForce(rbForwardForce, 0f);
            verticalForce = setupForce(rbUpwardsForce, 1f);

          
            //start drop after a few seconds
            StartCoroutine(playerDrop());
        }
        //standard movement
        else
        {
            verticalForce = 0f;
            sidewaysForce = 0f;
            forwardForce = setupForce(rbForwardForce, 1f);
       
            //if falling, push down for the vertical force
            if (isFalling)
            {
               verticalForce = -setupForce(rbDownwardsForce, 2f); 
            }
        }
        //assign force
        force.z = forwardForce;
        force.y = verticalForce;
        force.x = sidewaysForce;

        //add final force
        rb.AddForce(force, forceMode);
    }

    /// <summary>
    /// Determines if the player can jump or not (based on factors)
    /// </summary>
    /// <returns></returns>
    protected bool PlayerCanJump()
    {
        bool canJump = true;

        //dont trigger if in menu, in air or is a shape
        if (PauseMenu.GameIsPaused || isInAir || isThin || isFlat)
        {
            canJump = false;
        }
        return canJump; 
    }

    //if the player is changing the player shape (thin or flat), adjust it's size
    protected void HandlePlayerShapeTransform()
    {
    
        Vector3 playerScale = rb.transform.localScale;
        Vector3 playerPosition = rb.transform.localPosition;

        //Thin shape, taller but not as wide
        if (isThin)
        {
            playerScale.y = 2.0f;
            playerScale.x = 0.5f;
            playerPosition.y = 1.5f;

        }
        //Flat shape, not as tall but wider
        else if (isFlat)
        {
            playerScale.x = 2.0f;
            playerScale.y = 0.5f;
        }
        //Player is back to the regular shape
        else if (IsRegular)
        {
            playerScale.x = 1f;
            playerScale.y = 1f;
        }

        rb.transform.localScale = playerScale;
        rb.transform.localPosition = playerPosition;

    }

    //checks to ensure player is not out of bounds
    protected void HandlePlayerOutOfBounds()
    {
      
        float playerYPosition = transform.position.y;

        //if player has fallen off, finish game and play sound
        if (playerYPosition <= 0 && !IsFallingToDeath)
        {
            IsFallingToDeath = true;
            gameManager.EndGame();
        }

    }

    //Drop player after X seconds in air
    IEnumerator playerDrop()
    {
        yield return new WaitForSeconds(playerDropTime);
        isFalling = true; 
    }

    //Determines the forces based on time delta (and other factors)
    protected float setupForce(float baseForce, float factor = 1f) 
    {
        float force = (baseForce * factor) * Time.deltaTime;
        return force; 
    }


    //right click playermovement script and this appears down the bottom as a dynamic function
    [ContextMenu("Perform some Function")]
    void DoSomething()
    {
        Debug.Log("Perform something");
    }


}
