using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public int moveX = 1; // Used to determine whether the camera should follow the player on the x axis and the movement speed.
    public float displaceX = 1f; // Used to determine the x position of the object relative to the player.
    public float displaceY = 1f; // Used to determine the y position of the object relative to the player.
    public Transform followTransform;


    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = new Vector3(followTransform.position.x * moveX + displaceX, 
        followTransform.position.y + displaceY, 
        this.transform.position.z);
    }
}