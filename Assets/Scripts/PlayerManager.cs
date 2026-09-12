using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject canvas, undoManager;
    [SerializeField] LayerMask stopsMovementLayer, boxLayer;
    [SerializeField] AudioSource nonpersistentSoundSource;
    [SerializeField] AudioClip candySound, grassSound, waterSound, gameOverSound, winSound, starSound, hurtSound;
    LevelUI uiScript;
    UndoManager undoScript;

    public Transform movePoint;
    public bool finishedMovingInWater, isMakingMove, isFirstStarMove, hasStar; //isVerticalWater //justGotStar
    public int candy;
    public float horizontalInput, verticalInput, mobileHorizontalInput, mobileVerticalInput, waterHorizontalInput, waterVerticalInput; //1 = right or up, -1 = left or down
    public Sprite starPlayerSprite; //normalPlayerSprite //don't need normalPlayerSprite bc animator automatically shows normalPlayerSprite
    public SpriteRenderer playerSpriteRenderer;
    public Animator playerAnimator;

    Vector3 lastDirection;

    void Start()
    {
        uiScript = canvas.GetComponent<LevelUI>();
        undoScript = undoManager.GetComponent<UndoManager>();
        movePoint.parent = null;
        //finishedMovingInWater = true;
    }

    void Update()
    {
        if (finishedMovingInWater)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
            {
                transform.position = movePoint.position;
                if (isFirstStarMove)
                {
                    //will move in same direction as last time 
                    //if cannot move in same direction and is in water, get pushed by water
                    if (!Move(lastDirection) && (waterVerticalInput != 0 || waterHorizontalInput != 0))
                    {
                        finishedMovingInWater = false;
                        StartCoroutine(WaterMove(new Vector3(waterHorizontalInput * 1.5f, waterVerticalInput * 1.5f, 0)));
                    } else
                    {
                        waterVerticalInput = 0f;
                        waterHorizontalInput = 0f;
                    }
                    isFirstStarMove = false;
                } else
                {
                    if (isMakingMove)
                    {
                        undoScript.SetState();
                        isMakingMove = false;
                    }
                    if (uiScript.isPlaying)
                    {
                        if (!DataManager.Instance.isOnMobile)
                        {
                            horizontalInput = Input.GetAxisRaw("Horizontal");
                            verticalInput = Input.GetAxisRaw("Vertical");
                        } else
                        {
                            horizontalInput = mobileHorizontalInput;
                            verticalInput = mobileVerticalInput;
                        }

                        Vector3 moveDirection;

                        if (Mathf.Abs(horizontalInput) == 1f) // -1 or 1
                        {
                           moveDirection = new Vector3(horizontalInput * 1.5f, 0, 0); // multiply by input bc of -1 or 1
                        }
                        else if (Mathf.Abs(verticalInput) == 1f)
                        {
                            moveDirection = new Vector3(0, verticalInput * 1.5f, 0);
                        }
                        else
                        {
                            return;
                        }

                        if (Move(moveDirection))
                        {
                            undoScript.currentState.playerPos = transform.position;
                            if (hasStar) //
                            {
                                isFirstStarMove = true;
                            }
                            isMakingMove = true;
                            nonpersistentSoundSource.PlayOneShot(grassSound);
                            lastDirection = moveDirection;
                        }
                    }
                }
            }
        } else
        {
           if (waterVerticalInput != 0) // if (isVerticalWater) // if (verticalInput != 0)
            {
                if (transform.position.x != movePoint.position.x)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(movePoint.position.x, transform.position.y, 0), moveSpeed * Time.deltaTime);
                }
                else if (transform.position.y != movePoint.position.y)
                {
                    transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
                }
                else
                {
                    finishedMovingInWater = true;
                    waterVerticalInput = 0f;//
                }
            } else
            {
                if (transform.position.y != movePoint.position.y)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, movePoint.position.y, 0), moveSpeed * Time.deltaTime);
                }
                else if (transform.position.x != movePoint.position.x)
                {
                    transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
                }
                else
                {
                    finishedMovingInWater = true;
                    waterHorizontalInput = 0f;//
                }
            }
            
        }
    }

    bool Move(Vector3 changeInPosition)
    {
        bool boxMoved = true;
        Collider2D pushableCollider = Physics2D.OverlapCircle(movePoint.position + changeInPosition, .2f, boxLayer);
        if (pushableCollider)
        {
            if (finishedMovingInWater)
            {
                PushManager pushScript = pushableCollider.GetComponent<PushManager>();
                boxMoved = pushScript.Push(changeInPosition);
                if (boxMoved)
                {
                    undoScript.currentState.lastBox = pushableCollider.gameObject;
                }
            } else
            {
                return false;
            }
        }
        if (boxMoved && !Physics2D.OverlapCircle(movePoint.position + changeInPosition, .2f, stopsMovementLayer))
        {
            movePoint.position += changeInPosition;
            return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            if (hasStar)
            {
                nonpersistentSoundSource.PlayOneShot(hurtSound);
                Move(new Vector3(horizontalInput * (-1.5f), verticalInput * (-1.5f), 0));
                hasStar = false;
                isFirstStarMove = false;
                undoScript.currentState.starWasLost = true;
                //undoScript.currentState.playerPos = transform.position;
                playerAnimator.enabled = true;
                GameManager.Instance.FadeOutYellowVignette();
                //playerSpriteRenderer.sprite = normalPlayerSprite;
                //moveAmount = 1.5f;
                //undoScript.SetState();
                //isMakingMove = false;
            }
            else
            {
                undoScript.SetState();
                isMakingMove = false;
                nonpersistentSoundSource.PlayOneShot(gameOverSound);
                uiScript.ToggleDeathPanel();
                horizontalInput = 0f;
                verticalInput = 0f;
                gameObject.SetActive(false);
            }
        } else if (collision.gameObject.CompareTag("Candy"))
        {
            collision.gameObject.SetActive(false);
            undoScript.currentState.lastCandy = collision.gameObject;
            nonpersistentSoundSource.PlayOneShot(candySound);
            candy++;
        } else if (collision.gameObject.CompareTag("Star"))
        {
            collision.gameObject.SetActive(false);
            undoScript.currentState.lastStar = collision.gameObject;
            hasStar = true;
            nonpersistentSoundSource.PlayOneShot(starSound);
            playerAnimator.enabled = false;
            playerSpriteRenderer.sprite = starPlayerSprite;
            GameManager.Instance.FadeInYellowVignette();
        } else if (collision.gameObject.CompareTag("Flag"))
        {
            undoScript.SetState();
            isMakingMove = false;
            nonpersistentSoundSource.PlayOneShot(winSound);
            uiScript.ToggleWinPanel();
            gameObject.SetActive(false);
        }
        if (finishedMovingInWater && isMakingMove) //&& !isFirstStarMove
        {
            if (collision.gameObject.CompareTag("WaterDown"))
            {
                waterVerticalInput = -1f;
                waterHorizontalInput = 0f;
            } else if (collision.gameObject.CompareTag("WaterUp"))
            {
                waterVerticalInput = 1f;
                waterHorizontalInput = 0f;
            } else if (collision.gameObject.CompareTag("WaterLeft"))
            {
                waterVerticalInput = 0f;
                waterHorizontalInput = -1f;
            } else if (collision.gameObject.CompareTag("WaterRight"))
            {
                waterVerticalInput = 0f;
                waterHorizontalInput = 1f;
            } else
            {
                return;
            }

            if (!isFirstStarMove)
            {
                finishedMovingInWater = false;
                StartCoroutine(WaterMove(new Vector3(waterHorizontalInput * 1.5f, waterVerticalInput * 1.5f, 0)));
            }

            /*
            if (collision.gameObject.CompareTag("WaterDown"))
            {
                //isVerticalWater = true;
                verticalInput = -1f;
                horizontalInput = 0f;
                StartCoroutine(WaterMove(new Vector3(0, -1.5f, 0)));
            } else if (collision.gameObject.CompareTag("WaterUp"))
            {
                //isVerticalWater = true;
                verticalInput = 1f;
                horizontalInput = 0f;
                StartCoroutine(WaterMove(new Vector3(0, 1.5f, 0)));
            } else if (collision.gameObject.CompareTag("WaterLeft"))
            {
                //isVerticalWater = false;
                verticalInput = 0f;
                horizontalInput = -1f;
                StartCoroutine(WaterMove(new Vector3(-1.5f, 0, 0)));
            } else if (collision.gameObject.CompareTag("WaterRight"))
            {
                //isVerticalWater = false;
                verticalInput = 0f;
                horizontalInput = 1f;
                StartCoroutine(WaterMove(new Vector3(1.5f, 0, 0)));
            } 
            */
        }
    }

    IEnumerator WaterMove(Vector3 waterChangeInPosition)
    {
        yield return new WaitForSeconds(0.1f);
        if (Move(waterChangeInPosition))
        {
            nonpersistentSoundSource.PlayOneShot(waterSound);
        }
    }
}
