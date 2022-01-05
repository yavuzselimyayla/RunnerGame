using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    
    private GameObject player;              //current player (square)
    public Vector3 cameraPlayerOffset;      // Offset of the camera from the player
    public float lerpTime = 0.1f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    //On update, set the camera position to be position of the player
    void FixedUpdate()
    {

        Transform playerPosition = player.transform;
        //position the camera the same as the player but with an offset
        Vector3 trgPos = playerPosition.position + cameraPlayerOffset;
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(trgPos.x, cameraPlayerOffset.y, trgPos.z),
            0.1f);
    }
}
