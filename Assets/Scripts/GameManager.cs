using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // global singleton that any script can access by saying "GameManager.Instance.MyMethod()"
    public static GameManager Instance;

    public string currentScene;

    [SerializeField] GameObject levelCamera, levelCanvas, sceneTransitionCanvas, undoManager;
    [SerializeField] AudioClip buttonSound, mainMenuMusic, levelMusic;
    [SerializeField] Transform movePoint;
    [SerializeField] Image sceneFade, yellowVignette;
    [SerializeField] float visualFadeDuration;
    public AudioSource persistentSoundSource, nonpersistentSoundSource, mainMenuMusicSource, levelMusicSource;
    public GameObject player;
    public Vector3 spawnPos;
    public bool isLevelSelect, isSwitchingScene;

    PlayerManager playerScript;
    public LevelUI levelUIScript;
    public CameraFollow cameraFollowScript;
    public UndoManager undoScript;

    public SpriteRenderer hat, face, body;

    private void Awake()
    {
        // ensure only one GameManager instance exists
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        //Just do these in inspector:
        /*
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        currentScene = "MainMenu";
        DisableCoreScene();
        */

        playerScript = player.GetComponent<PlayerManager>();
        levelUIScript = levelCanvas.GetComponent<LevelUI>();
        cameraFollowScript = levelCamera.GetComponent<CameraFollow>();
        undoScript = undoManager.GetComponent<UndoManager>();
    }

    public void DisableCoreScene()
    {
        levelCanvas.SetActive(false);
    }

    public void EnableCoreScene()
    {
        levelCanvas.SetActive(true);
        //ResetLevel(); // SpawnPoint script already does this
    }

    /*
    public void SwitchScene(string newScene)
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
        currentScene = newScene;
        levelUIScript.sceneName = currentScene;
        nonpersistentSoundSource.Stop();
    }
    */

    public void SwitchScene(string newScene)
    {
        if (isSwitchingScene)
            return;

        StartCoroutine(SwitchSceneCoroutine(newScene));
    }

    IEnumerator SwitchSceneCoroutine(string newScene)
    {
        isSwitchingScene = true;

        sceneTransitionCanvas.SetActive(true);

        persistentSoundSource.PlayOneShot(buttonSound);
        nonpersistentSoundSource.Stop();

        // Fade in black
        yield return StartCoroutine(VisualFade(0f, 1f, sceneFade));

        // Start both operations without waiting for either one
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);

        // Now wait until both are finished
        while (!unloadOperation.isDone || !loadOperation.isDone)
        {
            yield return null;
        }

        // Fade out black
        yield return StartCoroutine(VisualFade(1f, 0f, sceneFade));

        currentScene = newScene;
        levelUIScript.sceneName = currentScene;

        sceneTransitionCanvas.SetActive(false);

        isSwitchingScene = false;
    }

    IEnumerator VisualFade(float startAlpha, float endAlpha, Image target)
    {
        float elapsed = 0f;
        while (elapsed < visualFadeDuration)
        {
            elapsed += Time.deltaTime;
            //Mathf.Lerp() is linear interpolation which lets you quickly and smoothly move a value from one number to another
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / visualFadeDuration);
            Color color = target.color;
            color.a = alpha;
            target.color = color;
            yield return null;
        }
        Color finalColor = target.color;
        finalColor.a = endAlpha;
        target.color = finalColor;
    }

    public void FadeInYellowVignette()
    {
        yellowVignette.gameObject.SetActive(true);
        StartCoroutine(GameManager.Instance.VisualFade(0f, 0.25f, yellowVignette));
    }

    public void FadeOutYellowVignette()
    {
        StartCoroutine(GameManager.Instance.VisualFade(0.25f, 0f, yellowVignette));
        yellowVignette.gameObject.SetActive(false);
    }

    public void PlayMainMenuMusic()
    {
        StartCoroutine(MusicCrossfade(levelMusicSource, mainMenuMusicSource));
    }

    public void PlayLevelMusic()
    {
        StartCoroutine(MusicCrossfade(mainMenuMusicSource, levelMusicSource));
    }
    
    IEnumerator MusicCrossfade(AudioSource oldSource, AudioSource newSource)
    {
        newSource.Play();

        float time = 0f;
        float maxMusicVolume = DataManager.Instance.musicVolume;

        while (time < 1f) // 1f is fade duration. Can change it to any value
        {
            time += Time.deltaTime;

            float t = time / 1f;

            oldSource.volume = (1f - t) * maxMusicVolume;
            newSource.volume = t * maxMusicVolume;

            yield return null; // needed for gradual crossFade (instead of all in one frame)
        }

        oldSource.Stop();
        oldSource.volume = 0f;
        newSource.volume = maxMusicVolume;
    }

    public void ResetLevel() // only called by SpawnPoint script so that spawn point exists before this happens
    {
        undoScript.Setup();

        playerScript.enabled = false; //

        player.transform.position = spawnPos;
        movePoint.position = spawnPos;

        playerScript.finishedMovingInWater = false;
        //playerScript.isVerticalWater = false;
        playerScript.isMakingMove = false;
        playerScript.candy = 0;
        playerScript.hasStar = false;
        playerScript.isFirstStarMove = false;
        playerScript.justGotStar = false;
        playerScript.horizontalInput = 0f;
        playerScript.verticalInput = 0f;
        playerScript.playerAnimator.enabled = true;
        //playerScript.playerSpriteRenderer.sprite = playerScript.normalPlayerSprite;

        levelUIScript.movedAfterUndo = false;
        levelUIScript.isPlaying = true;
        levelUIScript.currentScore = 0;
        levelUIScript.currentTime = 0f;
        levelUIScript.died = false;
        levelUIScript.won = false;

        levelUIScript.originalPanel.SetActive(true);
        levelUIScript.deathPanel.SetActive(false);
        levelUIScript.winPanel.SetActive(false);
        levelUIScript.settingsPanel.SetActive(false);
        levelUIScript.infoPanel.SetActive(false);
        levelUIScript.restartConfirmPanel.SetActive(false);
        levelUIScript.restartConfirmBG.SetActive(false);
        //levelUIScript.mobilePanel.SetActive(); // done in LevelUI update
        levelUIScript.Setup();

        yellowVignette.gameObject.SetActive(false);

        // Should make much cleaner Setup() methods for LevelUI, MainMenuUI, and PlayerManager
        // also make UI scripts only focus on UI, moving the Undo logic away from it

        levelMusicSource.UnPause();

        player.SetActive(true);
        playerScript.gameObject.SetActive(true);
        playerScript.enabled = true;
    }
}
