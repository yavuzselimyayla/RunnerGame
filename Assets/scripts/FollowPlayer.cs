using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    //current player (square)
    private GameObject player;

    //offset of the camera from the player
    public Vector3 cameraPlayerOffset;

    private void Start()
    {
        player = GameObject.FindWithTag("Player"); 
    }

    //On update, set the camera position to be position of the player
    void Update () {

        Transform playerPosition = player.transform;
        //position the camera the same as the player but with an offset
        transform.position = playerPosition.position + cameraPlayerOffset;
    }
}
