using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
//using UnityEngine.Device;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject currentPanel, currentSubPanel, settingsPanel, levelPanel, cosmeticPanel, soundButton, musicButton, timerButton, quitButton, mainMenuBG;
    [SerializeField] TextMeshProUGUI[] scoreTextArr, timerAnyTextArr, timerHundredTextArr;
    [SerializeField] TextMeshProUGUI timerToggleText, mobileToggleText;
    [SerializeField] Sprite soundOnSprite, soundOffSprite, musicOnSprite, musicOffSprite, timerOnSprite, timerOffSprite;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] Slider soundSlider, musicSlider;
    [SerializeField] ScrollRect levelSelectScrollRect;
    [SerializeField] Toggle restartConfirmSettingsToggle;
    [SerializeField] CosmeticManager cosmeticScript;

    AudioSource persistentSoundSource, nonpersistentSoundSource, mainMenuMusicSource, levelMusicSource;
    GameObject previousPanel, previousSubPanel;
    bool hasChangedSettings;

    void Start()
    {
        persistentSoundSource = GameManager.Instance.persistentSoundSource;
        nonpersistentSoundSource = GameManager.Instance.nonpersistentSoundSource;//
        mainMenuMusicSource = GameManager.Instance.mainMenuMusicSource;
        levelMusicSource = GameManager.Instance.levelMusicSource;//

        mainMenuMusicSource.mute = DataManager.Instance.isMusicMute;
        levelMusicSource.mute = DataManager.Instance.isMusicMute;//
        if (mainMenuMusicSource.mute)
        {
            musicButton.GetComponent<Image>().sprite = musicOffSprite;
        } else
        {
            musicButton.GetComponent<Image>().sprite = musicOnSprite;
        }
        persistentSoundSource.mute = DataManager.Instance.isSoundMute;
        nonpersistentSoundSource.mute = DataManager.Instance.isSoundMute;//
        if (persistentSoundSource.mute)
        {
            soundButton.GetComponent<Image>().sprite = soundOffSprite;
        } else
        {
            soundButton.GetComponent<Image>().sprite = soundOnSprite;
        }
        mainMenuMusicSource.volume = DataManager.Instance.musicVolume;
        levelMusicSource.volume = DataManager.Instance.musicVolume;//
        musicSlider.value = DataManager.Instance.musicVolume;
        persistentSoundSource.volume = DataManager.Instance.soundVolume;
        nonpersistentSoundSource.volume = DataManager.Instance.soundVolume;//
        soundSlider.value = DataManager.Instance.soundVolume;

        restartConfirmSettingsToggle.SetIsOnWithoutNotify(!DataManager.Instance.doNotShowConfirm);

        if (DataManager.Instance.isTimer)
        {
            foreach (TextMeshProUGUI timerText in timerAnyTextArr)
            {
                timerText.gameObject.SetActive(true);
            }
            foreach (TextMeshProUGUI timerText in timerHundredTextArr)
            {
                timerText.gameObject.SetActive(true);
            }
            timerButton.GetComponent<Image>().sprite = timerOnSprite;
        }

        for (int i = 0; i < DataManager.Instance.numberOfLevels; i++)
        {
            scoreTextArr[i].text = DataManager.Instance.scoreArr[i].ToString() + "/5";

            TimeSpan time = TimeSpan.FromSeconds(DataManager.Instance.timeAnyArr[i]);
            if (time.TotalMinutes >= 1)
            {
                timerAnyTextArr[i].text = "ANY% " + time.ToString(@"m\:ss\.ff");
            }
            else
            {
                timerAnyTextArr[i].text = "ANY% " + time.ToString(@"s\.ff");
            }
            time = TimeSpan.FromSeconds(DataManager.Instance.timeHundredArr[i]);
            if (time.TotalMinutes >= 1)
            {
                timerHundredTextArr[i].text = "100% " + time.ToString(@"m\:ss\.ff");
            }
            else
            {
                timerHundredTextArr[i].text = "100% " + time.ToString(@"s\.ff");
            }
        }

        if (DataManager.Instance.isTimer)
        {
            timerToggleText.text = "Timer on";
        } else
        {
            timerToggleText.text = "Timer off";
        }

        if (Application.isMobilePlatform || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            DataManager.Instance.isOnMobile = true;
        }

        levelSelectScrollRect.horizontalNormalizedPosition = DataManager.Instance.GetLevelSelectPos();

        if (GameManager.Instance.isLevelSelect)
        {
            ChangePanel(levelPanel);
        }

        hasChangedSettings = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf)
            {
                ReturnToPreviousPanel();
            }
            else
            {
                ChangePanel(settingsPanel);
            }
        }

        DataManager.Instance.musicVolume = mainMenuMusicSource.volume;
        DataManager.Instance.soundVolume = persistentSoundSource.volume;

        if (DataManager.Instance.isOnMobile)
        {
            mobileToggleText.text = "Mobile: on";
        } else
        {
            mobileToggleText.text = "Mobile: off";
        }
    }

    public void ChangePanel(GameObject newPanel)
    {
        currentPanel.SetActive(false);
        if (newPanel == levelPanel)
        {
            if (!GameManager.Instance.isLevelSelect)
            {
                persistentSoundSource.PlayOneShot(buttonSound);
            }
            mainMenuBG.SetActive(false);
        } else
        {
            persistentSoundSource.PlayOneShot(buttonSound);
            if (!mainMenuBG.activeSelf)
            {
                mainMenuBG.SetActive(true);
            }
        }
        previousPanel = currentPanel;
        currentPanel = newPanel;
        newPanel.SetActive(true);
    }

    public void ChangeSubPanel(GameObject newSubPanel)
    {
        currentSubPanel.SetActive(false);
        persistentSoundSource.PlayOneShot(buttonSound);
        previousSubPanel = currentSubPanel;
        currentSubPanel = newSubPanel;
        newSubPanel.SetActive(true);
    }

    public void ReturnToPreviousPanel()
    {
        currentPanel.SetActive(false);
        persistentSoundSource.PlayOneShot(buttonSound);
        if (currentPanel == levelPanel)
        {
            mainMenuBG.SetActive(true);
        } else if ((currentPanel == settingsPanel) && hasChangedSettings)
        {
            hasChangedSettings = false;
            DataManager.Instance.SaveSettings();
            //DataManager.Instance.SaveMobileState();
        } else if ((currentPanel == cosmeticPanel) && cosmeticScript.hasChangedCosmetics)
        {
            cosmeticScript.SaveCurrentCosmetics();
        }
        currentPanel = previousPanel;
        previousPanel.SetActive(true);
    }

    public void ReturnToPreviousSubPanel()
    {
        currentSubPanel.SetActive(false);
        persistentSoundSource.PlayOneShot(buttonSound);
        currentSubPanel = previousSubPanel;
        previousSubPanel.SetActive(true);
    }

    public void PlayLevel(string levelName)
    {
        DataManager.Instance.SaveLevelSelectPos(levelSelectScrollRect.horizontalNormalizedPosition);
        GameManager.Instance.PlayLevelMusic();
        GameManager.Instance.SwitchScene(levelName);
        GameManager.Instance.EnableCoreScene();
    }

    public void ToggleTimer()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        if (DataManager.Instance.isTimer)
        {
            foreach (TextMeshProUGUI timerText in timerAnyTextArr)
            {
                timerText.gameObject.SetActive(false);
            }
            foreach (TextMeshProUGUI timerText in timerHundredTextArr)
            {
                timerText.gameObject.SetActive(false);
            }
            timerToggleText.gameObject.SetActive(true);
            timerToggleText.text = "Timer off";
            timerButton.GetComponent<Image>().sprite = timerOffSprite;
            DataManager.Instance.isTimer = false;
        } else
        {
            foreach (TextMeshProUGUI timerText in timerAnyTextArr)
            {
                timerText.gameObject.SetActive(true);
            }
            foreach (TextMeshProUGUI timerText in timerHundredTextArr)
            {
                timerText.gameObject.SetActive(true);
            }
            timerToggleText.text = "Timer on";
            timerButton.GetComponent<Image>().sprite = timerOnSprite;
            DataManager.Instance.isTimer = true;
        }
        hasChangedSettings = true;
    }

    public void ToggleSoundEffects()
    {
        if (persistentSoundSource.mute)
        {
            soundButton.GetComponent<Image>().sprite = soundOnSprite;
            persistentSoundSource.mute = false;
            nonpersistentSoundSource.mute = false;//
            DataManager.Instance.isSoundMute = false;
        } else
        {
            soundButton.GetComponent<Image>().sprite = soundOffSprite;
            persistentSoundSource.mute = true;
            nonpersistentSoundSource.mute = true;//
            DataManager.Instance.isSoundMute = true;
        }
        hasChangedSettings = true;
    }

    public void ToggleMusic()
    {
        if (mainMenuMusicSource.mute)
        {
            musicButton.GetComponent<Image>().sprite = musicOnSprite;
            mainMenuMusicSource.mute = false;
            levelMusicSource.mute = false;//
            DataManager.Instance.isMusicMute = false;
        } else
        {
            musicButton.GetComponent<Image>().sprite = musicOffSprite;
            mainMenuMusicSource.mute = true;
            levelMusicSource.mute = true;//
            DataManager.Instance.isMusicMute = true;
        }
        hasChangedSettings = true;
    }

    public void OnSoundSliderChanged()
    {
        persistentSoundSource.volume = soundSlider.value;
        nonpersistentSoundSource.volume = soundSlider.value;//
        DataManager.Instance.soundVolume = soundSlider.value;
        hasChangedSettings = true;
    }

    public void OnMusicSliderChanged()
    {
        mainMenuMusicSource.volume = musicSlider.value;
        levelMusicSource.volume = musicSlider.value;//
        DataManager.Instance.musicVolume = musicSlider.value;
        hasChangedSettings = true;
    }

    public void ToggleMobileMode()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        DataManager.Instance.isOnMobile = !DataManager.Instance.isOnMobile;
        if (DataManager.Instance.isOnMobile)
        {
            mobileToggleText.text = "Mobile: on";
            Screen.fullScreen = true;
        } else
        {
            mobileToggleText.text = "Mobile: off";
        }
        DataManager.Instance.SaveMobileState();
    }

    public void ToggleRestartConfirmSettings()
    {
        DataManager.Instance.doNotShowConfirm = !DataManager.Instance.doNotShowConfirm;
        hasChangedSettings = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}