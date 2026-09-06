using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectKeyboardControl : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] float scrollSpeed;
    [SerializeField] bool isVertical;

    void Update()
    {
        if (isVertical)
        {
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                scrollRect.verticalNormalizedPosition += scrollSpeed * Time.deltaTime;
            }
        } else
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                scrollRect.horizontalNormalizedPosition -= scrollSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                scrollRect.horizontalNormalizedPosition += scrollSpeed * Time.deltaTime;
            }
        }
    }
}