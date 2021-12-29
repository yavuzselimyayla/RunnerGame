using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Material))]
public class CoinComponent : MonoBehaviour {

    //master gamemanager class
    protected GameManager gameManager;

    //audio manager instance for sounds
    protected AudioManager audioManager;

    //Animator object to trigger animations for the coin
    protected Animator coinAnimator;

    //renderer for the coin
    protected Renderer coinRenderer;

    [Header("Coin Information")]
    [Tooltip("'standard' for normal coins and 'special' for big coins. Determines animation to play")]
    public string coinName = "standard";
    public int coinValue = 10;
    [Tooltip("The name for the sound to be played on collection, will play a sound on AudioManager")]
    public string CoinSoundName;

    //determines if the coin is animating or not
    [Header("Animation States")]
    public bool isCoinAnimatingToActive = false;
    public bool isCoinAnimatingToInactive = false;

    [Header("Materials")]
    public Material coinMaterialInactive;
    public Material coinMaterialActive;
    [Tooltip("The time it takes to animate from the inactive to active material")]
    public float coinMaterialChangeDuration = 0.1f;



    // Use this for initialization
    void Start () {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = AudioManager.instance;
        coinRenderer = GetComponent<Renderer>();
        coinAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Updates every frame (updates animation)
    /// </summary>
    private void Update()
    {
        //update coin animation
        UpdateCoinAnimationMaterial(); 
    }

    /// <summary>
    /// Triggered when object is collided with
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider collidedElement)
    {
        string collidedTag = collidedElement.tag;
        if(collidedElement.tag == "Player")
        {
            CollidedWithPlayer(collidedElement);
        }
        
    }

    /// <summary>
    /// Called when the player collides with the coin
    /// Triggers coin collect sound, triggers animation and eventually destroys
    /// </summary>
    protected void CollidedWithPlayer(Collider player)
    { 
        //Start the pickup sound
        PlayCoinPickupSound();

        //Play coin active animation (and trigger removal 
        PlayCoinActiveAnimation();

        //update the players score with coin value
        UpdatePlayerScore();

        //Destroy the coin after a second
        StartCoroutine("DestroyCoin", 1f);

    }

    /// <summary>
    /// Play the sound of picking up the coin
    /// </summary>
    protected void PlayCoinPickupSound()
    {
        audioManager.Play(CoinSoundName);
    }

    /// <summary>
    /// Triggers the animation on the coin
    /// </summary>
    protected void PlayCoinActiveAnimation()
    {
        //play standard animation (jump up and scale out)
        if(coinName == "standard")
        {
            coinAnimator.Play("coinActiveAnimation");
        }
        //special coin animation (scale out and drop)
        else if(coinName == "special" || coinName == "ultra")
        {
            coinAnimator.Play("coinSpecialAnimation");
        }
       
    }

 

    //set that coin is animating from inactive to active state
    protected void ChangeCoinMaterialToActive()
    {
  //      Debug.LogWarning("Coin will move from inactive to active");
        isCoinAnimatingToActive = true;
        isCoinAnimatingToInactive = false; 
    }

    //set that coin is animating from active to inactive state
    protected void ChangeCoinMaterialToInactive()
    {
    //    Debug.LogWarning("Coin will move from active to inactive");
        isCoinAnimatingToActive = false;
        isCoinAnimatingToInactive = true;
    }

    /// <summary>
    /// handles the animation from one material to another (based on the curreent state of the animation)
    /// 
    /// Will animate from the active material to inactive and vice versa based on animation state
    /// </summary>
    protected void UpdateCoinAnimationMaterial()
    {

    
        //Animate from current material to the active material
        if (isCoinAnimatingToActive)
        {
            Material currentMaterial = coinRenderer.material; 
            float lerp = Mathf.PingPong(Time.time, coinMaterialChangeDuration) / coinMaterialChangeDuration;
            coinRenderer.material.Lerp(currentMaterial, coinMaterialActive, lerp);
        }
        //animate from current material to the inactive (regular) material
        else if (isCoinAnimatingToInactive)
        {
            Material currentMaterial = coinRenderer.material;
            float lerp = Mathf.PingPong(Time.time, coinMaterialChangeDuration) / coinMaterialChangeDuration;
            coinRenderer.material.Lerp(currentMaterial, coinMaterialInactive, lerp);
        }
    }

    /// <summary>
    /// Destroys the coin, called once the pickup has been completed
    /// </summary>
    protected IEnumerator DestroyCoin()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    /// <summary>
    /// Calls the gamemanager and tells it to update the score
    /// </summary>
    protected void UpdatePlayerScore()
    {
        gameManager.UpdateScore(coinValue); 
    }


}
