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