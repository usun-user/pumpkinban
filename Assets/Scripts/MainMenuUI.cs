using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
//using UnityEngine.Device;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject originalPanel, settingsPanel, levelPanel, cosmeticsPanel, cosmeticSectionPanel, hatPanel, facePanel, bodyPanel, skinPanel, soundButton, musicButton, timerButton, quitButton, mainMenuBG; //infoPanel
    [SerializeField] TextMeshProUGUI[] scoreTextArr, timerAnyTextArr, timerHundredTextArr;
    [SerializeField] TextMeshProUGUI timerToggleText, mobileToggleText;
    [SerializeField] Sprite soundOnSprite, soundOffSprite, musicOnSprite, musicOffSprite, timerOnSprite, timerOffSprite, normalPlayerSkin, selectedButtonSprite, unselectedButtonSprite;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] Slider soundSlider, musicSlider;
    [SerializeField] ScrollRect levelSelectScrollRect;
    [SerializeField] Toggle restartConfirmSettingsToggle;
    [SerializeField] Image hat, face, body, skin;

    AudioSource persistentSoundSource, nonpersistentSoundSource, mainMenuMusicSource, levelMusicSource;

    //public float currentTime = 0f;

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
            //timerTextArr[i].text = TimeSpan.FromSeconds(DataManager.Instance.timeArr[i]).ToString(@"mm\:ss\.ff");

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

        levelSelectScrollRect.horizontalNormalizedPosition = DataManager.Instance.levelSelectPos;

        if (GameManager.Instance.isLevelSelect)
        {
            StartLevelSelect();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf)
            {
                ReturnToGame();
            }
            else
            {
                ToggleSettingsPanel();
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

    public void ToggleSettingsPanel()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        mainMenuBG.SetActive(true);
        originalPanel.SetActive(false);
        //infoPanel.SetActive(false);
        settingsPanel.SetActive(true);
        levelPanel.SetActive(false);
        cosmeticsPanel.SetActive(false);
    }

    /*
    public void ToggleInfoPanel()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        originalPanel.SetActive(false);
        infoPanel.SetActive(true);
        settingsPanel.SetActive(false);
        levelPanel.SetActive(false);
        cosmeticsPanel.SetActive(false);
    }
    */

    public void ToggleCosmeticsPanel()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        mainMenuBG.SetActive(true);
        originalPanel.SetActive(false);
        //infoPanel.SetActive(false);
        settingsPanel.SetActive(false);
        levelPanel.SetActive(false);
        cosmeticsPanel.SetActive(true);
        cosmeticSectionPanel.SetActive(true);
    }

    public void ToggleHatPanel()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        cosmeticSectionPanel.SetActive(false);
        hatPanel.SetActive(true);
    }

    public void ToggleFacePanel()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        cosmeticSectionPanel.SetActive(false);
        facePanel.SetActive(true);
    }

    public void ToggleBodyPanel()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        cosmeticSectionPanel.SetActive(false);
        bodyPanel.SetActive(true);
    }

    public void ToggleSkinPanel()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        cosmeticSectionPanel.SetActive(false);
        skinPanel.SetActive(true);
    }

    public void ChangeCosmeticItem(Sprite newCosmeticItem)
    {
        if (hatPanel.activeSelf)
        {
            if (hat.sprite == newCosmeticItem)
            {
                hat.enabled = false;
                GameManager.Instance.hat.enabled = false;
            } 
            else
            {
                hat.sprite = newCosmeticItem;
                hat.enabled = true;
                GameManager.Instance.hat.sprite = newCosmeticItem;
            }
        } 
        else if (facePanel.activeSelf)
        {
            if (face.sprite == newCosmeticItem)
            {
                face.enabled = false;
                GameManager.Instance.face.enabled = false;
            }
            else
            {
                face.sprite = newCosmeticItem;
                face.enabled = true;
                GameManager.Instance.face.sprite = newCosmeticItem;
            }
        }
        else if (bodyPanel.activeSelf)
        {
            if (body.sprite == newCosmeticItem)
            {
                body.enabled = false;
                GameManager.Instance.body.enabled = false;
            }
            else
            {
                body.sprite = newCosmeticItem;
                body.enabled = true;
                GameManager.Instance.body.sprite = newCosmeticItem;
            }
        }
        else if(skinPanel.activeSelf)
        {
            if (skin.sprite == newCosmeticItem)
            {
                skin.sprite = normalPlayerSkin;
                GameManager.Instance.skin.sprite = normalPlayerSkin;
            }
            else
            {
                skin.sprite = newCosmeticItem;
                GameManager.Instance.skin.sprite = newCosmeticItem;
            }
        }
    }

    public void ClearAllCosmetics()
    {
        hat.enabled = false;
        GameManager.Instance.hat.enabled = false;
        face.enabled = false;
        GameManager.Instance.face.enabled = false;
        body.enabled = false;
        GameManager.Instance.body.enabled = false;
        skin.sprite = normalPlayerSkin;
    }

    /*
    public void ToggleLevelPanel()
    {
        soundSource.PlayOneShot(buttonSound);
        levelPanel.SetActive(true);
        originalPanel.SetActive(false);
        infoPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    */
    /*
    public void ReturnToSettings()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        //infoPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    */
    public void ReturnToGame()
    {
        persistentSoundSource.PlayOneShot(buttonSound);
        mainMenuBG.SetActive(true);
        settingsPanel.SetActive(false);
        //infoPanel.SetActive(false);
        originalPanel.SetActive(true);
        levelPanel.SetActive(false);
        cosmeticsPanel.SetActive(false);
    }

    public void PlayLevel(string levelName)
    {
        DataManager.Instance.levelSelectPos = levelSelectScrollRect.horizontalNormalizedPosition;
        GameManager.Instance.PlayLevelMusic();
        GameManager.Instance.SwitchScene(levelName);
        GameManager.Instance.EnableCoreScene();
    }

    public void StartLevelSelect()
    {
        if (!GameManager.Instance.isLevelSelect)
        {
            persistentSoundSource.PlayOneShot(buttonSound);
        }
        levelPanel.SetActive(true);
        originalPanel.SetActive(false);
        mainMenuBG.SetActive(false);
        /*
        if (DataManager.Instance.isOnMobile)
        {
            GameManager.Instance.SetupLevelSelect(true);
        }
        else
        {
            GameManager.Instance.SetupLevelSelect(false);
        }
        */
    }

    /*
    IEnumerator SceneChange(string newScene)
    {
        yield return new WaitForSeconds(buttonSound.length);
        GameManager.Instance.SwitchScene(newScene);
        GameManager.Instance.EnableCoreScene();
        //SceneManager.LoadScene(newScene);
    }
    */

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
    }

    public void OnSoundSliderChanged()
    {
        persistentSoundSource.volume = soundSlider.value;
        nonpersistentSoundSource.volume = soundSlider.value;//
        DataManager.Instance.soundVolume = soundSlider.value;
    }

    public void OnMusicSliderChanged()
    {
        mainMenuMusicSource.volume = musicSlider.value;
        levelMusicSource.volume = musicSlider.value;//
        DataManager.Instance.musicVolume = musicSlider.value;
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
    }

    public void ToggleRestartConfirmSettings()
    {
        DataManager.Instance.doNotShowConfirm = !DataManager.Instance.doNotShowConfirm;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}