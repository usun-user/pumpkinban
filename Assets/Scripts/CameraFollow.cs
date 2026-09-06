using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;

    private Vector3 tempPos;

    public float minX, maxX; // LevelBounds script decides this through inspector

    [SerializeField] GameObject player;

    public static CameraFollow instance;

    private void Awake()
    {
        if (CameraFollow.instance == null) instance = this;
        else Destroy(gameObject);
    }

    void LateUpdate()
    {
        playerTransform = player.GetComponent<Transform>();

        if (!playerTransform)
            return;

        tempPos = transform.position;
        tempPos.x = playerTransform.position.x;

        if (tempPos.x < minX)
            tempPos.x = minX;

        if (tempPos.x > maxX)
            tempPos.x = maxX;

        transform.position = tempPos;
    }
}
