using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLoop : MonoBehaviour
{
    /*
    BoxCollider2D boxCollider;
    Rigidbody2D rb;
    float width;
    public float speed;
    public int numberOfTiles;
    public bool isVertical;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (isVertical)
        {
            width = boxCollider.size.y;
            rb.velocity = new Vector2(0, speed);
        } else
        {
            width = boxCollider.size.x;
            rb.velocity = new Vector2(speed, 0);
        }
    }

    void Update()
    {
        if (speed > 0)
        {
            if (isVertical)
            {
                if (transform.position.y > (width * numberOfTiles / 2))
                {
                    Vector2 vector = new Vector2(0, width * numberOfTiles);
                    transform.position = (Vector2)transform.position - vector;
                }
            }
            else if (transform.position.x > (width * numberOfTiles / 2))
            {
                Vector2 vector = new Vector2(width * numberOfTiles, 0);
                transform.position = (Vector2)transform.position - vector;
            }
        } else
        {
            if (isVertical)
            {
                if (transform.position.y < -(width * numberOfTiles / 2))
                {
                    Vector2 vector = new Vector2(0, width * numberOfTiles);
                    transform.position = (Vector2)transform.position + vector;
                }
            }
            else if (transform.position.x < -(width * numberOfTiles / 2))
            {
                Vector2 vector = new Vector2(width * numberOfTiles, 0);
                transform.position = (Vector2)transform.position + vector;
            }
        }
    }
    */
}
