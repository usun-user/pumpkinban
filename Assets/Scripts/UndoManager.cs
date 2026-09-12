using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.EventSystems;

public class UndoManager : MonoBehaviour
{
    [SerializeField] GameObject canvas, player, originalPanel, deathPanel, winPanel, settingsPanel, infoPanel;
    [SerializeField] AudioSource persistentSoundSource, nonpersistentSoundSource, levelMusicSource;
    [SerializeField] AudioClip buttonSound;

    public bool movedAfterUndo;
    public Button[] undoButtonArr;

    LevelUI uiScript;
    PlayerManager playerScript;

    public GameState currentState;
    Stack<GameState> history;

    Vector3 previousPos;

    Coroutine undoCoroutine;

    void Start()
    {
        uiScript = canvas.GetComponent<LevelUI>();
        playerScript = player.GetComponent<PlayerManager>();
        Setup();
    }

    public void Setup()
    {
        foreach (Button undoButton in undoButtonArr)
        {
            undoButton.interactable = false;
        }
        history = new Stack<GameState>();
        currentState = new GameState();
    }

    public void SetState()
    {
        // Doesn't work for some reason
        /*
        if (previousPos == currentState.playerPos 
            && !currentState.lastCandy 
            && !currentState.lastBox 
            && !currentState.lastStar) //&& !currentState.lastWeb
        {
            return;
        }
        previousPos = currentState.playerPos; 
        */
        
        history.Push(currentState);
        currentState = new GameState();
        movedAfterUndo = true;

        if (history.Count == 1)
        {
            foreach (Button undoButton in undoButtonArr)
            {
                undoButton.interactable = true;
            }
        }
    }

    public void Undo()
    {
        if ((!undoButtonArr[0].interactable) || playerScript.isMakingMove)
        {
            return;
        }

        currentState = new GameState();

        foreach (Button undoButton in undoButtonArr)
        {
            undoButton.interactable = false;
        }

        GameState lastState = history.Pop();
        GameObject lastCandy = lastState.lastCandy;
        GameObject lastBox = lastState.lastBox;
        GameObject lastWeb = lastState.lastWeb;
        GameObject lastStar = lastState.lastStar;
        Vector3 playerPos = lastState.playerPos;
        Vector3 boxPos = lastState.boxPos;

        playerScript.enabled = false;
        playerScript.gameObject.SetActive(true);
        uiScript.isPlaying = false;

        if (uiScript.won || uiScript.died)
        {
            uiScript.won = false;
            uiScript.died = false;
            nonpersistentSoundSource.Stop();
        }

        persistentSoundSource.PlayOneShot(buttonSound);

        playerScript.movePoint.position = playerPos;
        playerScript.gameObject.transform.position = playerPos;

        if (lastBox)
        {
            PushManager pushScript = lastBox.GetComponent<PushManager>();
            pushScript.enabled = false;
            lastBox.SetActive(true);

            pushScript.boxMovePoint.position = boxPos;
            lastBox.transform.position = boxPos;

            pushScript.enabled = true;
        }

        if (lastCandy)
        {
            playerScript.candy--;
            lastCandy.SetActive(true);
        }
        if (lastWeb)
        {
            lastWeb.SetActive(true);
        }
        if (lastStar)
        {
            //playerScript.moveAmount = 1.5f;
            playerScript.hasStar = false;
            playerScript.isFirstStarMove = false;
            //playerScript.justGotStar = false;
            lastStar.SetActive(true);
            playerScript.playerAnimator.enabled = true;
            GameManager.Instance.FadeOutYellowVignette();
            //playerScript.playerSpriteRenderer.sprite = playerScript.normalPlayerSprite;
        } else if (lastState.starWasLost)
        {
            playerScript.hasStar = true;
            playerScript.isFirstStarMove = false;
            //playerScript.justGotStar = true;
            playerScript.playerAnimator.enabled = false;
            playerScript.playerSpriteRenderer.sprite = playerScript.starPlayerSprite;
            GameManager.Instance.FadeInYellowVignette();
        }

        if (deathPanel.activeSelf)
        {
            deathPanel.SetActive(false);
            levelMusicSource.UnPause();
        }
        else if (winPanel.activeSelf)
        {
            winPanel.SetActive(false);
            levelMusicSource.UnPause();
        }

        infoPanel.SetActive(false);
        settingsPanel.SetActive(false);
        originalPanel.SetActive(true);

        uiScript.isPlaying = true;
        playerScript.enabled = true;
        movedAfterUndo = false;

        if (history.Count > 0)
        {
            foreach (Button undoButton in undoButtonArr)
            {
                undoButton.interactable = true;
            }
        } else
        {
            currentState.playerPos = GameManager.Instance.spawnPos;
        }
    }

    public void Down()
    {
        if (undoCoroutine == null) //
        {
            undoCoroutine = StartCoroutine(UndoWhileHeld());
        }
    }

    public void Up()
    {
        if (undoCoroutine != null)
        {
            StopCoroutine(undoCoroutine);
            undoCoroutine = null;
        }
    }

    private IEnumerator UndoWhileHeld()
    {
        // First undo immediately
        Undo();

        // Wait before repeating
        yield return new WaitForSeconds(0.5f);

        // Repeat while held
        while (true)
        {
            Undo();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

public class GameState
{
    public GameObject lastCandy, lastBox, lastWeb, lastStar;
    public Vector3 playerPos, boxPos;
    public bool starWasLost;
}