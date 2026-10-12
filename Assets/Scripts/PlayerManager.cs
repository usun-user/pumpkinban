using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] LayerMask stopsMovementLayer, boxLayer, webBadgeLevelLayer, waterBadgeLevelLayer, starBadgeLevelLayer;
    [SerializeField] AudioSource nonpersistentSoundSource, levelMusicSource;
    [SerializeField] AudioClip candySound, grassSound, waterSound, gameOverSound, winSound, starSound, hurtSound;
    [SerializeField] LevelUI uiScript;
    [SerializeField] UndoManager undoScript;
    [SerializeField] Sprite starPlayerSprite; //don't need normalPlayerSprite bc animator automatically shows normalPlayerSprite
    [SerializeField] SpriteRenderer playerSpriteRenderer, hatRenderer, faceRenderer, bodyRenderer;

    public Transform movePoint;
    public bool isMovingInWater, isMakingMove, isFirstStarMove, hasStar;
    public float horizontalInput, verticalInput, mobileHorizontalInput, mobileVerticalInput; //1 = right/up, -1 = left/down
    public Animator playerAnimator;

    Vector3 lastInputDirection, lastGeneralDirection, pendingWaterDirection;
    bool isPlaying, isSpikeMove, pendingWaterPush, waterMoveStarted; //isSpikeMove is needed to prevent extra water push after spike bounce
    int candy;

    Coroutine waterCoroutine;

    void Start()
    {
        movePoint.parent = null;
    }

    public void Setup()
    {
        isMovingInWater = false;
        waterMoveStarted = false;
        pendingWaterPush = false;
        pendingWaterDirection = Vector3.zero;
        isMakingMove = false;
        candy = 0;
        hasStar = false;
        isFirstStarMove = false;
        isSpikeMove = false;
        horizontalInput = 0f;
        verticalInput = 0f;
        mobileHorizontalInput = 0f;
        mobileVerticalInput = 0f;
        playerAnimator.enabled = true;
        SetPlayerVisibility(true);

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

        if (!isMovingInWater)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
            {
                transform.position = movePoint.position;
                
                // start the pending water push only after reaching the tile
                // let the star-move logic resolve first
                if (pendingWaterPush && !isFirstStarMove && !isSpikeMove)
                {
                    Vector3 direction = pendingWaterDirection;
                    pendingWaterPush = false;

                    StartWaterPush(direction);
                    return;
                }

                if (isSpikeMove)
                {
                    isSpikeMove = false;
                }
                else if (isFirstStarMove)
                {
                    isFirstStarMove = false;
                    
                    if (!Move(lastInputDirection))
                    {
                        if (pendingWaterPush)
                        {
                            pendingWaterPush = false;

                            StartWaterPush(pendingWaterDirection);
                            return;
                        }
                    }
                    else
                    {
                        // The box was successfully pushed
                        pendingWaterPush = false;
                    }
                    
                }
                else
                {
                    if (isMakingMove)
                    {
                        undoScript.SetState();
                        isMakingMove = false;
                    }

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
                        lastInputDirection = moveDirection;
                    }
                }
            }
        } else if (waterMoveStarted)
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
                    waterMoveStarted = false;
                    isMovingInWater = false;
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
                    waterMoveStarted = false;
                    isMovingInWater = false;
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
            if (!isMovingInWater)
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
            lastGeneralDirection = changeInPosition;
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
                isMovingInWater = false;
                waterMoveStarted = false;
                pendingWaterPush = false;
                pendingWaterDirection = Vector3.zero;
                isSpikeMove = true;

                nonpersistentSoundSource.PlayOneShot(hurtSound);
                Move(-lastGeneralDirection);
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
                SetPlayerActive(false);
                SetPlayerVisibility(false);
            }
        } else if (collision.gameObject.CompareTag("Candy"))
        {
            collision.gameObject.SetActive(false);
            undoScript.currentState.lastCandy = collision.gameObject;
            nonpersistentSoundSource.PlayOneShot(candySound);
            //candy++;
            uiScript.UpdateScoreText(++candy); // ++ before candy means that candy is updated before it is given to UpdateScoreText method
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
            if (Physics2D.OverlapCircle(movePoint.position, .2f, webBadgeLevelLayer))
            {
                BadgeManager.Instance.WinBadge(6);
            }
            if (Physics2D.OverlapCircle(movePoint.position, .2f, waterBadgeLevelLayer))
            {
                BadgeManager.Instance.WinBadge(7);
            }
            if (Physics2D.OverlapCircle(movePoint.position, .2f, starBadgeLevelLayer))
            {
                BadgeManager.Instance.WinBadge(8);
            }

            undoScript.SetState();
            isMakingMove = false;
            nonpersistentSoundSource.PlayOneShot(winSound);
            uiScript.ToggleWinPanel();
            SetPlayerActive(false);
            SetPlayerVisibility(false);
        }
        
        if (collision.gameObject.CompareTag("InSecret"))
        {
            levelMusicSource.Pause();
            BadgeManager.Instance.WinBadge(10);
        } else if (collision.gameObject.CompareTag("OutSecret"))
        {
            levelMusicSource.UnPause();
        }

        bool isWater =
            collision.CompareTag("WaterDown") ||
            collision.CompareTag("WaterUp") ||
            collision.CompareTag("WaterLeft") ||
            collision.CompareTag("WaterRight");

        if (isWater && !isSpikeMove && !isMovingInWater && isMakingMove)
        {
            Vector3 direction = Vector3.zero;

            if (collision.CompareTag("WaterDown"))
                direction = Vector3.down * 1.5f;
            else if (collision.CompareTag("WaterUp"))
                direction = Vector3.up * 1.5f;
            else if (collision.CompareTag("WaterLeft"))
                direction = Vector3.left * 1.5f;
            else if (collision.CompareTag("WaterRight"))
                direction = Vector3.right * 1.5f;

            pendingWaterPush = true;
            pendingWaterDirection = direction;
        }
    }

    void StartWaterPush(Vector3 direction)
    {
        if (isSpikeMove || isMovingInWater || !isMakingMove)
            return;

        isMovingInWater = true;
        waterMoveStarted = false;
        isFirstStarMove = false;

        horizontalInput = Mathf.Sign(direction.x);
        verticalInput = Mathf.Sign(direction.y);

        waterCoroutine = StartCoroutine(WaterMove(direction));
    }

    IEnumerator WaterMove(Vector3 waterChangeInPosition)
    {
        pendingWaterPush = false;

        if (isSpikeMove || !isMakingMove)
        {
            isMovingInWater = false;
            waterMoveStarted = false;
            waterCoroutine = null;
            yield break;
        }

        if (Move(waterChangeInPosition))
        {
            waterMoveStarted = true;
            nonpersistentSoundSource.PlayOneShot(waterSound);
        }
        else
        {
            isMovingInWater = false;
            waterMoveStarted = false;
        }

        waterCoroutine = null;
        yield break;
    }

    public void SetPlayerActive(bool isActive)
    {
        isPlaying = isActive;
    }

    public void SetPlayerVisibility(bool isActive)
    {
        playerSpriteRenderer.enabled = isActive;
        hatRenderer.enabled = isActive;
        faceRenderer.enabled = isActive;
        bodyRenderer.enabled = isActive;
    }

    public void SetStarFace()
    {
        playerSpriteRenderer.sprite = starPlayerSprite;
    }

    public void DecrementCandy()
    {
        uiScript.UpdateScoreText(--candy);
    }
}
