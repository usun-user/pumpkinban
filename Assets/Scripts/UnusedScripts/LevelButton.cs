using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelButton : MonoBehaviour
{
    /*
    public int levelNumber;
    public LevelButton[] nextLevels;

    [SerializeField] Sprite lockedSprite, unlockedSprite;
    [SerializeField] TextMeshProUGUI scoreText, anyPercentTimeText, hundredPercentTimeText, levelNameText;

    public void PlayLevel()
    {
        GameManager.Instance.SwitchScene("Level" + levelNumber);
        GameManager.Instance.EnableCoreScene();
    }

    public void UnlockNextLevels()
    {
        foreach (LevelButton nextLevel in nextLevels)
        {
            nextLevel.UnlockSelf();
        }
    }

    public void UnlockSelf()
    {
        nextLevel.gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().sprite = unlockedSprite;
        levelNameText.SetActive(true);
    }

    public void UpdateSelf()
    {
        scoreText.text = DataManager.Instance.scoreArr[levelNumber - 1].ToString() + "/5";
        timerTextArr[i].text = "-- " + TimeSpan.FromSeconds(DataManager.Instance.timeArr[i]).ToString(@"mm\:ss\.ff");
    }
    */

    /*
        public string levelSceneName;
        public string levelID; // instead of relying on name?
        public LevelButton[] nextLevels;

        [SerializeField] Sprite lockedSprite, unlockedSprite;
        [SerializeField] TextMeshProUGUI scoreText, timeText, levelNameText;
        [SerializeField] AudioClip buttonSound;

        public static DataManager Instance;

        // Will get info to display in text from DataManager
        /*
        public int bestScore;
        public float bestTimeAnyCandies;
        public float bestTimeAllCandies;
        */

    /*
        public void UnlockNextLevels()
        {
            foreach (LevelButton nextLevel in nextLevels)
            {
                nextLevel.gameObject.SetActive(true);
            }
        }

        public void UpdateInfo()
        {
            int i = 0; // temporary
            // either replace levelSceneName with level index, so each level is named "Level#"
            // OR replace scoreArr and timeArr in DataManager with Dictionary (HashMap in C#), and access info with levelSceneName
            scoreText.text = DataManager.Instance.scoreArr[i];
        }

        public void ToggleInfo()
        {
            // when you click special button to show extra info about level (ex. time)
        }

        // Will replace method in UIManager?
        public void PlayLevel()
        {
            soundSource.PlayOneShot(buttonSound);
            StartCoroutine(SceneChange());
        }

        IEnumerator SceneChange()
        {
            yield return new WaitForSeconds(buttonSound.length);
            SceneManager.LoadScene(levelSceneName);
        }
    */
}
