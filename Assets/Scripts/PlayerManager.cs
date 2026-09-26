using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] LayerMask stopsMovementLayer, boxLayer;
    [SerializeField] AudioSource nonpersistentSoundSource;
    [SerializeField] AudioClip candySound, grassSound, waterSound, gameOverSound, winSound, starSound, hurtSound;
    //[SerializeField] BoxCollider2D playerCollider;
    [SerializeField] LevelUI uiScript;
    [SerializeField] UndoManager undoScript;

    public Transform movePoint;
    public bool isMovingInWater, isMakingMove, isFirstStarMove, hasStar, isSpikeMove; //isSpikeMove is needed to prevent extra water push after spike bounce
    public int candy;
    public float horizontalInput, verticalInput, mobileHorizontalInput, mobileVerticalInput; //1 = right/up, -1 = left/down
    public Sprite starPlayerSprite; //don't need normalPlayerSprite bc animator automatically shows normalPlayerSprite
    public SpriteRenderer playerSpriteRenderer;
    public Animator playerAnimator;

    Vector3 lastDirection;
    bool isPlaying;

    Coroutine waterCoroutine;

    void Start()
    {
        movePoint.parent = null;
    }

    public void Setup()
    {
        isMovingInWater = false; //finishedMovingInWater = true;
        isMakingMove = false;
        candy = 0;
        hasStar = false;
        isFirstStarMove = false;
        isSpikeMove = false;//
        horizontalInput = 0f;
        verticalInput = 0f;
        mobileHorizontalInput = 0f;
        mobileVerticalInput = 0f;
        playerAnimator.enabled = true;
        playerSpriteRenderer.enabled = true;

        if (waterCoroutine != null)
        {
            StopCoroutine(waterCoroutine);
            waterCoroutine = null;
        }

        SetPlayerActive(true);
    }

    void Update()
    {
        if (!isPlaying)
            return;

        if (!isMovingInWater) //if(finishedMovingInWater)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
            {
                transform.position = movePoint.position;
                
                if (isSpikeMove) //
                {
                    isSpikeMove = false;//
                }
                else if (isFirstStarMove)
                {
                    isFirstStarMove = false;
                    Move(lastDirection); //move in same direction as last time
                }
                else
                {
                    if (isMakingMove)
                    {
                        undoScript.SetState();
                        isMakingMove = false;
                    }
                    //if (uiScript.isPlaying)
                    //{
                    if (!DataManager.Instance.isOnMobile)
                    {
                        horizontalInput = Input.GetAxisRaw("Horizontal");
                        verticalInput = Input.GetAxisRaw("Vertical");
                    }
                    else
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
                        if (hasStar)
                        {
                            isFirstStarMove = true;
                        }
                        isMakingMove = true;
                        nonpersistentSoundSource.PlayOneShot(grassSound);
                        lastDirection = moveDirection;
                    }
                    //}
                }
            }
        } else
        {
           if (verticalInput != 0)
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
                    isMovingInWater = false; //finishedMovingInWater = true;
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
                    isMovingInWater = false; //finishedMovingInWater = true;
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
            if (!isMovingInWater) //finishedMovingInWater
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
        if (!isPlaying)
            return;

        if (collision.gameObject.CompareTag("Spike"))
        {
            if (hasStar)
            {
                if (waterCoroutine != null)
                {
                    StopCoroutine(waterCoroutine);
                    waterCoroutine = null;
                }
                //finishedMovingInWater = true;
                isMovingInWater = false;
                isSpikeMove = true;

                nonpersistentSoundSource.PlayOneShot(hurtSound);
                //Move(new Vector3(horizontalInput * (-1.5f), verticalInput * (-1.5f), 0));
                Move(-lastDirection);
                hasStar = false;
                isFirstStarMove = false;
                undoScript.currentState.starWasLost = true;
                playerAnimator.enabled = true;
                GameManager.Instance.FadeOutYellowVignette();

                //return; //Cannot return bc would stop new water movement for if player ends up on water after spike bounce
            }
            else
            {
                undoScript.SetState();
                isMakingMove = false;
                nonpersistentSoundSource.PlayOneShot(gameOverSound);
                uiScript.ToggleDeathPanel();
                mobileHorizontalInput = 0f;
                mobileVerticalInput = 0f;
                horizontalInput = 0f;
                verticalInput = 0f;
                //gameObject.SetActive(false);
                SetPlayerActive(false);
                playerSpriteRenderer.enabled = false;
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
            //gameObject.SetActive(false);
            SetPlayerActive(false);
            playerSpriteRenderer.enabled = false;
        }
        //finishedMovingInWater
        if (!isSpikeMove && !isMovingInWater && isMakingMove && (!isFirstStarMove || Physics2D.OverlapCircle(movePoint.position + lastDirection, .2f, stopsMovementLayer)))
        {
            if (collision.gameObject.CompareTag("WaterDown"))
            {
                verticalInput = -1f;
                horizontalInput = 0f;
            } else if (collision.gameObject.CompareTag("WaterUp"))
            {
                verticalInput = 1f;
                horizontalInput = 0f;
            } else if (collision.gameObject.CompareTag("WaterLeft"))
            {
                verticalInput = 0f;
                horizontalInput = -1f;
            } else if (collision.gameObject.CompareTag("WaterRight"))
            {
                verticalInput = 0f;
                horizontalInput = 1f;
            } else
            {
                return;
            }

            //finishedMovingInWater = false;
            isMovingInWater = true;
            isFirstStarMove = false;
            waterCoroutine = StartCoroutine(WaterMove(new Vector3(horizontalInput * 1.5f, verticalInput * 1.5f, 0)));
        }
    }

    IEnumerator WaterMove(Vector3 waterChangeInPosition)
    {
        yield return new WaitForSeconds(0.1f);
        if (Move(waterChangeInPosition))
        {
            nonpersistentSoundSource.PlayOneShot(waterSound);
        }
        waterCoroutine = null;
    }

    public void SetPlayerActive(bool isActive)
    {
        isPlaying = isActive;
        //playerCollider.enabled = isActive;
    }
}
