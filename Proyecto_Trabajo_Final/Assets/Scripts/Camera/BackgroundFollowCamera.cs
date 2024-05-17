using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour
{
    public Transform m_CameraToFollow;
    public float m_VerticalOffset = 0.0f;

    // Update is called once per frame
    void LateUpdate()
    {
        // Follow the camera on the y position
        transform.position = new Vector3(transform.position.x, m_CameraToFollow.position.y - m_VerticalOffset, transform.position.z);
    }
}