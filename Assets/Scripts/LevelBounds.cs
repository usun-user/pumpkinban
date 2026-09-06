using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    [SerializeField] float minX, maxX;

    CameraFollow cameraFollowScript;

    void Start()
    {
        cameraFollowScript = GameManager.Instance.cameraFollowScript;
        cameraFollowScript.minX = minX;
        cameraFollowScript.maxX = maxX;
    }
}
