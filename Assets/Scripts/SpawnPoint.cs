using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.spawnPos = this.transform.position;
        GameManager.Instance.ResetLevel();
    }
}
