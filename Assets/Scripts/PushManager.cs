using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushManager : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    public Transform boxMovePoint;
    [SerializeField] LayerMask stopsMovementLayer;
    AudioSource nonpersistentSoundSource;
    [SerializeField] AudioClip boxDestroyedSound, webDestroyedSound;
    UndoManager undoScript;

    void Start()
    {
        nonpersistentSoundSource = GameManager.Instance.nonpersistentSoundSource;
        undoScript = GameManager.Instance.undoScript;
        boxMovePoint.parent = null;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, boxMovePoint.position, moveSpeed * Time.deltaTime);
    }

    public bool Push(Vector3 changeInPosition)
    {
        if (!Physics2D.OverlapCircle(boxMovePoint.position + changeInPosition, .2f, stopsMovementLayer))
        {
            undoScript.currentState.boxPos = boxMovePoint.position; //

            boxMovePoint.position += changeInPosition;
            return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            nonpersistentSoundSource.PlayOneShot(boxDestroyedSound);
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Web"))
        {
            undoScript.currentState.lastWeb = collision.gameObject;
            nonpersistentSoundSource.PlayOneShot(webDestroyedSound);
            collision.gameObject.SetActive(false);
        }
    }
}
