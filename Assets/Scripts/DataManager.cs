using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DataManager : MonoBehaviour
{
    // global singleton that any script can access by saying "DataManager.Instance.MyMethod()"
    public static DataManager Instance;

    public int numberOfLevels;

    public bool isTimer, isMusicMute, isSoundMute, isOnMobile, doNotShowConfirm, firstTimePlaying;
    public float musicVolume, soundVolume;
    public int[] scoreArr;
    public float[] timeAnyArr, timeHundredArr;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (PlayerPrefs.HasKey("levelSelectPos")) // If user has played before
        {
            isTimer = (PlayerPrefs.GetInt("isTimer") == 1);
            isMusicMute = (PlayerPrefs.GetInt("isMusicMute") == 1);
            isSoundMute = (PlayerPrefs.GetInt("isSoundMute") == 1);
            isOnMobile = (PlayerPrefs.GetInt("isOnMobile") == 1); // Do not put in "else" bc isOnMobile is set in MainMenuUI
            doNotShowConfirm = (PlayerPrefs.GetInt("doNotShowConfirm") == 1);
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
            soundVolume = PlayerPrefs.GetFloat("soundVolume");
        } else
        {
            firstTimePlaying = true;
            PlayerPrefs.SetInt("isTimer", 0);
            PlayerPrefs.SetInt("isMusicMute", 0);
            PlayerPrefs.SetInt("isSoundMute", 0);
            PlayerPrefs.SetInt("doNotShowConfirm", 0);
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
            PlayerPrefs.SetFloat("soundVolume", 0.5f);
            PlayerPrefs.SetFloat("levelSelectPos", 0f);
        }
        for (int i = 0; i < numberOfLevels; i++)
        {
            string levelIndex = i.ToString();
            if (PlayerPrefs.HasKey("score" + levelIndex))
            {
                scoreArr[i] = PlayerPrefs.GetInt("score" + levelIndex);
                timeAnyArr[i] = PlayerPrefs.GetFloat("timeAny" + levelIndex);
                timeHundredArr[i] = PlayerPrefs.GetFloat("timeHundred" + levelIndex);
            } else
            {
                PlayerPrefs.SetInt("score" + levelIndex, 0);
                PlayerPrefs.SetFloat("timeAny" + levelIndex, 3599);
                PlayerPrefs.SetFloat("timeHundred" + levelIndex, 3599);
            }
        }
        PlayerPrefs.Save();
    }
    
    public void SaveLevel()
    {
        for (int i = 0; i < numberOfLevels; i++)
        {
            string levelIndex = i.ToString();
            PlayerPrefs.SetInt("score" + levelIndex, scoreArr[i]);
            PlayerPrefs.SetFloat("timeAny" + levelIndex, timeAnyArr[i]);
            PlayerPrefs.SetFloat("timeHundred" + levelIndex, timeHundredArr[i]);
        }
        PlayerPrefs.Save();
    }

    public CosmeticSprites GetCosmeticSprites()
    {
        return new CosmeticSprites
        {
            newHatIndex = PlayerPrefs.GetInt("hatIndex"),
            newFaceIndex = PlayerPrefs.GetInt("faceIndex"),
            newBodyIndex = PlayerPrefs.GetInt("bodyIndex")
        };
    }

    public void SaveCosmetics(int newHatIndex, int newFaceIndex, int newBodyIndex)
    {
        PlayerPrefs.SetInt("hatIndex", newHatIndex);
        PlayerPrefs.SetInt("faceIndex", newFaceIndex);
        PlayerPrefs.SetInt("bodyIndex", newBodyIndex);

        PlayerPrefs.Save();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("isTimer", Convert.ToInt32(isTimer));
        PlayerPrefs.SetInt("isMusicMute", Convert.ToInt32(isMusicMute));
        PlayerPrefs.SetInt("isSoundMute", Convert.ToInt32(isSoundMute));
        PlayerPrefs.SetInt("isOnMobile", Convert.ToInt32(isOnMobile));//
        PlayerPrefs.SetInt("doNotShowConfirm", Convert.ToInt32(doNotShowConfirm));
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("soundVolume", soundVolume);

        PlayerPrefs.Save();
    }

    public void SaveLevelSelectPos(float levelSelectPos)
    {
        PlayerPrefs.SetFloat("levelSelectPos", levelSelectPos);
        PlayerPrefs.Save();
    }

    public void SaveMobileState()
    {
        PlayerPrefs.SetInt("isOnMobile", Convert.ToInt32(isOnMobile));
        PlayerPrefs.Save();
    }

    public void SaveRestartConfirm()
    {
        PlayerPrefs.SetInt("doNotShowConfirm", Convert.ToInt32(doNotShowConfirm));
        PlayerPrefs.Save();
    }

    public float GetLevelSelectPos()
    {
        return PlayerPrefs.GetFloat("levelSelectPos");
    }
}

public class CosmeticSprites
{
    public int newHatIndex, newFaceIndex, newBodyIndex;
}
