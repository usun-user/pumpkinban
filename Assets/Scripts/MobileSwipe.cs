using System.Collections;
using UnityEngine;

public class MobileSwipe : MonoBehaviour
{
    [SerializeField] float minSwipeDistance = 40f;
    [SerializeField] PlayerManager playerScript;

    private Vector2 startTouchPosition, endTouchPosition;
    private bool coroutineAllowed;

    private void Start()
    {
        coroutineAllowed = true;
    }

    private void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPosition = touch.position;
        }

        if (touch.phase == TouchPhase.Ended && coroutineAllowed)
        {
            endTouchPosition = touch.position;

            Vector2 swipe = endTouchPosition - startTouchPosition;

            // Ignore very small movements
            if (swipe.magnitude < minSwipeDistance)
                return;

            // Determine whether the swipe is horizontal or vertical
            if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
            {
                if (swipe.x > 0)
                {
                    StartCoroutine(Go(1f, 0f)); // Right
                }
                else
                {
                    StartCoroutine(Go(-1f, 0f)); // Left
                }
            }
            else
            {
                if (swipe.y > 0)
                {
                    StartCoroutine(Go(0f, 1f)); // Up
                }
                else
                {
                    StartCoroutine(Go(0f, -1f)); // Down
                }
            }
        }
    }

    private IEnumerator Go(float horizontalInput, float verticalInput)
    {
        coroutineAllowed = false;

        playerScript.mobileHorizontalInput = horizontalInput;
        playerScript.mobileVerticalInput = verticalInput;

        yield return new WaitForSeconds(0.05f);

        playerScript.mobileHorizontalInput = 0f;
        playerScript.mobileVerticalInput = 0f;

        coroutineAllowed = true;
    }
}

/*
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
*/