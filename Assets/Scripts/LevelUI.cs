using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
//using UnityEngine.Device;

public class LevelUI : MonoBehaviour
{
    public GameObject originalPanel, deathPanel, winPanel, settingsPanel, infoPanel, mobilePanel, restartConfirmPanel, restartConfirmBG; // use "[SerializeField]" instead of "public" in future by making Setup() here that doesn't need GameManager
    [SerializeField] GameObject player, soundButton, musicButton, timerButton, mobileButton, infoButton, undoManager, yellowVignetteObj;
    [SerializeField] TextMeshProUGUI[] scoreTextArr, timerTextArr;
    [SerializeField] TextMeshProUGUI timerToggleText;
    [SerializeField] Sprite soundOnSprite, soundOffSprite, musicOnSprite, musicOffSprite, timerOnSprite, timerOffSprite, mobileOnSprite, mobileOffSprite;
    [SerializeField] AudioSource persistentSoundSource, nonpersistentSoundSource, levelMusicSource, mainMenuMusicSource;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] Slider soundSlider, musicSlider;
    [SerializeField] Toggle restartConfirmSettingsToggle, doNotRestartConfirmToggle;

    PlayerManager playerScript;
    UndoManager undoScript;
    
    public bool movedAfterUndo, isPlaying = true;
    public int currentScore;
    public float currentTime = 0f;

    public bool died, won;
    public string sceneName;

    int levelIndex;

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        sceneName = GameManager.Instance.currentScene;
        levelMusicSource.mute = DataManager.Instance.isMusicMute;
        mainMenuMusicSource.mute = DataManager.Instance.isMusicMute;
        if (levelMusicSource.mute)
        {
            musicButton.GetComponent<Image>().sprite = musicOffSprite;
        } else
        {
            musicButton.GetComponent<Image>().sprite = musicOnSprite;
        }
        persistentSoundSource.mute = DataManager.Instance.isSoundMute;
        nonpersistentSoundSource.mute = DataManager.Instance.isSoundMute;
        if (persistentSoundSource.mute)
        {
            soundButton.GetComponent<Image>().sprite = soundOffSprite;
        } else
        {
            soundButton.GetComponent<Image>().sprite = soundOnSprite;
        }
        mainMenuMusicSource.volume = DataManager.Instance.musicVolume;
        levelMusicSource.volume = DataManager.Instance.musicVolume;
        musicSlider.value = DataManager.Instance.musicVolume;
        persistentSoundSource.volume = DataManager.Instance.soundVolume;
        nonpersistentSoundSource.volume = DataManager.Instance.soundVolume;
        soundSlider.value = DataManager.Instance.soundVolume;

        restartConfirmSettingsToggle.SetIsOnWithoutNotify(!DataManager.Instance.doNotShowConfirm);

        if (DataManager.Instance.isTimer)
        {
            timerToggleText.gameObject.SetActive(false);
            foreach (TextMeshProUGUI timerText in timerTextArr)
            {
                timerText.gameObject.SetActive(true);
            }
            timerButton.GetComponent<Image>().sprite = timerOnSprite;
        }

        playerScript = player.GetComponent<PlayerManager>();
        undoScript = undoManager.GetComponent<UndoManager>();
    }

    void Update()
    {
        if (restartConfirmPanel.activeSelf) // if restartConfirmPanel is active, stop player from toggling anything else with key presses
        {
            if (Input.GetKeyDown(KeyCode.Y)) 
            {
                ConfirmRestart();
            } else if (Input.GetKeyDown(KeyCode.N))
            {
                RejectRestart();
            }
        } else if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        } else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf)
            {
                ReturnToGame();
            } else
            {
                ToggleSettingsPanel();
            }
        } else if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Z))
        {
            undoScript.Down();
        } else if (Input.GetKeyUp(KeyCode.U) || Input.GetKeyUp(KeyCode.Z))
        {
            undoScript.Up();
        } else if (won && (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.Return)))
        {
            NextLevel();
        }
        /*
        else if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Z))
        {
            undoScript.Undo();
        } 
        */

        currentScore = playerScript.candy;
        scoreTextArr[0].text = currentScore.ToString() + "/5";
        scoreTextArr[1].text = currentScore.ToString() + "/5 candies";
        scoreTextArr[2].text = currentScore.ToString() + "/5 candies";
        if (isPlaying)
        {
            currentTime += Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        foreach (TextMeshProUGUI timerText in timerTextArr)
        {
            //timerText.text = time.ToString(@"mm\:ss\.ff");
            if (time.TotalMinutes >= 1)
            {
                timerText.text = time.ToString(@"m\:ss\.ff");
            }
            else
            {
                timerText.text = time.ToString(@"s\.ff");
            }
        }

        //Shouldn't need bc already updated in Setup() and On_SliderChanged()
        //DataManager.Instance.musicVolume = levelMusicSource.volume;
        //DataManager.Instance.soundVolume = persistentSoundSource.volume;

        // move this away from Update() and instead make it a result of button click
        if (DataManager.Instance.isOnMobile)
        {
            mobilePanel.SetActive(true);
            mobileButton.GetComponent<Image>().sprite = mobileOnSprite;
        } else
        {
            mobilePanel.SetActive(false);
            mobileButton.GetComponent<Image>().sprite = mobileOffSprite;
        }
    }

    public void ToggleDeathPanel()
    {
        isPlaying = false;
        died = true;
        levelMusicSource.Pause();
        originalPanel.SetActive(false);
        deathPanel.SetActive(true);
        winPanel.SetActive(false);
        infoPanel.SetActive(false);
        settingsPanel.SetActive(false);
        //yellowVignetteObj.SetActive(false); //can't die with star powerup, so this isn't needed
    }

    public void ToggleWinPanel()
    {
        isPlaying = false;
        won = true;
        levelMusicSource.Pause();
        levelIndex = sceneName[sceneName.Length - 1] - '0' - 1;
        if (currentScore > DataManager.Instance.scoreArr[levelIndex])
        {
            DataManager.Instance.scoreArr[levelIndex] = currentScore;
        }
        /*
        if (currentTime < DataManager.Instance.timeArr[levelIndex])
        {
            DataManager.Instance.timeArr[levelIndex] = currentTime;
        }
        */
        if (currentScore == 5 && currentTime < DataManager.Instance.timeHundredArr[levelIndex])
        {
            DataManager.Instance.timeHundredArr[levelIndex] = currentTime;
        }
        if (currentTime < DataManager.Instance.timeAnyArr[levelIndex]) // NOT else if
        {
            DataManager.Instance.timeAnyArr[levelIndex] = currentTime;
        }
        DataManager.Instance.SaveGame();
        originalPanel.SetActive(false);
        deathPanel.SetActive(false);
        winPanel.SetActive(true);
        infoPanel.SetActive(false);
        settingsPanel.SetActive(false);
        yellowVignetteObj.SetActive(false);
    }

    public void ToggleSettingsPanel()
    {
        isPlaying = false;
        persistentSoundSource.PlayOneShot(buttonSound);
        originalPanel.SetActive(false);
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
        infoPanel.SetActive(false);
        settingsPanel.SetActive(true);
        yellowVignetteObj.SetActive(false);
    }

    public void ToggleInfoPanel()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        originalPanel.SetActive(false);
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
        infoPanel.SetActive(true);
        settingsPanel.SetActive(false);
        yellowVignetteObj.SetActive(false);
    }

    public void ReturnToSettings()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        infoPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ReturnToGame()
    {
        if (!died && !won)
        {
            isPlaying = true;
        }
        persistentSoundSource.PlayOneShot(buttonSound);
        settingsPanel.SetActive(false);
        infoPanel.SetActive(false);
        if (died)
        {
            deathPanel.SetActive(true);
        } else if (won)
        {
            winPanel.SetActive(true);
        } else
        {
            originalPanel.SetActive(true);
        }
        if (playerScript.hasStar)
        {
            yellowVignetteObj.SetActive(true);
        }
    }

    public void RestartGame()
    {
        playerScript.enabled = false; // ? Do not SetActive(false) entire player because then cannot move to spawnPos
        if (DataManager.Instance.doNotShowConfirm)
        {
            GameManager.Instance.SwitchScene(sceneName);
        } else
        {
            doNotRestartConfirmToggle.SetIsOnWithoutNotify(false);
            if (!originalPanel.activeSelf)
            {
                restartConfirmBG.SetActive(true);
            }
            restartConfirmPanel.SetActive(true);
        }
    }

    public void ConfirmRestart()
    {
        restartConfirmPanel.SetActive(false);
        restartConfirmBG.SetActive(false);
        GameManager.Instance.SwitchScene(sceneName);
    }

    public void RejectRestart()
    {
        restartConfirmPanel.SetActive(false);
        restartConfirmBG.SetActive(false);
        playerScript.enabled = true;
    }

    public void ToggleRestartConfirmSettings()
    {
        DataManager.Instance.doNotShowConfirm = !DataManager.Instance.doNotShowConfirm;
    }

    public void ToggleDoNotShowConfirm()
    {
        DataManager.Instance.doNotShowConfirm = !DataManager.Instance.doNotShowConfirm;
        restartConfirmSettingsToggle.SetIsOnWithoutNotify(!restartConfirmSettingsToggle.isOn);
    }

    public void GoHome()
    {
        //soundSource.PlayOneShot(buttonSound);
        //GameManager.Instance.DisableCoreScene();
        player.SetActive(false);
        GameManager.Instance.PlayMainMenuMusic();
        GameManager.Instance.SwitchScene("MainMenu");
        GameManager.Instance.isLevelSelect = true;
        GameManager.Instance.DisableCoreScene(); // This LevelUI script is turned off
        //StartCoroutine(SceneChange("MainMenu"));
    }

    public void NextLevel()
    {
        if (levelIndex + 1 != DataManager.Instance.numberOfLevels)
        {
            //soundSource.PlayOneShot(buttonSound);
            player.SetActive(false);
            GameManager.Instance.SwitchScene("Level" + (levelIndex + 2).ToString());
            //StartCoroutine(SceneChange(levelName));
        }
    }

    /*
    IEnumerator SceneChange(string newScene)
    {
        yield return new WaitForSeconds(buttonSound.length);
        GameManager.Instance.SwitchScene(newScene);
        //SceneManager.LoadScene(newScene);
    }
    */

    public void ToggleTimer()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        if (DataManager.Instance.isTimer)
        {
            foreach (TextMeshProUGUI timerText in timerTextArr)
            {
                timerText.gameObject.SetActive(false);
            }
            timerToggleText.gameObject.SetActive(true);
            timerToggleText.text = "Timer off";
            timerButton.GetComponent<Image>().sprite = timerOffSprite;
            DataManager.Instance.isTimer = false;
        } else
        {
            foreach (TextMeshProUGUI timerText in timerTextArr)
            {
                timerText.gameObject.SetActive(true);
            }
            timerToggleText.gameObject.SetActive(false);
            timerButton.GetComponent<Image>().sprite = timerOnSprite;
            DataManager.Instance.isTimer = true;
        }
    }

    public void ToggleSoundEffects()
    {
        if (persistentSoundSource.mute)
        {
            soundButton.GetComponent<Image>().sprite = soundOnSprite;
            persistentSoundSource.mute = false;
            nonpersistentSoundSource.mute = false;
            DataManager.Instance.isSoundMute = false;
        } else
        {
            soundButton.GetComponent<Image>().sprite = soundOffSprite;
            persistentSoundSource.mute = true;
            nonpersistentSoundSource.mute = true;
            DataManager.Instance.isSoundMute = true;
        }
    }

    public void ToggleMusic()
    {
        if (levelMusicSource.mute)
        {
            musicButton.GetComponent<Image>().sprite = musicOnSprite;
            levelMusicSource.mute = false;
            mainMenuMusicSource.mute = false;
            DataManager.Instance.isMusicMute = false;
        } else
        {
            musicButton.GetComponent<Image>().sprite = musicOffSprite;
            levelMusicSource.mute = true;
            mainMenuMusicSource.mute = true;
            DataManager.Instance.isMusicMute = true;
        }
    }

    public void OnSoundSliderChanged()
    {
        persistentSoundSource.volume = soundSlider.value;
        nonpersistentSoundSource.volume = soundSlider.value;
        DataManager.Instance.soundVolume = soundSlider.value;
    }

    public void OnMusicSliderChanged()
    {
        mainMenuMusicSource.volume = musicSlider.value;
        levelMusicSource.volume = musicSlider.value;
        DataManager.Instance.musicVolume = musicSlider.value;
    }

    public void ToggleMobileMode()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        DataManager.Instance.isOnMobile = !DataManager.Instance.isOnMobile;
        if (DataManager.Instance.isOnMobile)
        {
            mobileButton.GetComponent<Image>().sprite = mobileOnSprite;
            Screen.fullScreen = true;
        } else
        {
            mobileButton.GetComponent<Image>().sprite = mobileOffSprite;
        }
    }
}