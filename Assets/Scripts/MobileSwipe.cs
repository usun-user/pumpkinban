using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileSwipe : MonoBehaviour
{
    private Vector2 startTouchPosition, endTouchPosition;

    private Touch touch;

    private IEnumerator goCoroutine;
    private bool coroutineAllowed;

    [SerializeField] GameObject player;
    PlayerManager playerScript;

    private void Start()
    {
        playerScript = player.GetComponent<PlayerManager>();
        coroutineAllowed = true;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPosition = touch.position;
        }

        if (Input.touchCount > 0 && touch.phase == TouchPhase.Ended && coroutineAllowed)
        {
            endTouchPosition = touch.position;

            if ((endTouchPosition.y > startTouchPosition.y) && (Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(touch.deltaPosition.x)))
            {
                goCoroutine = Go(0, 1f); // up
                StartCoroutine(goCoroutine);
            } else if ((endTouchPosition.y < startTouchPosition.y) && (Mathf.Abs(touch.deltaPosition.y) > Mathf.Abs(touch.deltaPosition.x)))
            {
                goCoroutine = Go(0, -1f); // down
                StartCoroutine(goCoroutine);
            } else if ((endTouchPosition.x < startTouchPosition.x) && (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))) 
            {
                goCoroutine = Go(-1f, 0); // left
                StartCoroutine(goCoroutine);
            } else if ((endTouchPosition.x > startTouchPosition.x) && (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y)))
            {
                goCoroutine = Go(1f, 0); // right
                StartCoroutine(goCoroutine);
            }
        }
    }

    private IEnumerator Go(float thisHorizontalInput, float thisVerticalInput)
    {
        coroutineAllowed = false;

        playerScript.mobileHorizontalInput = thisHorizontalInput;
        playerScript.mobileVerticalInput = thisVerticalInput;

        yield return new WaitForSeconds(0.05f); //originally was 0.01f but I think 0.05f is prob safer

        playerScript.mobileHorizontalInput = 0f;
        playerScript.mobileVerticalInput = 0f;

        coroutineAllowed = true;
    }
}
