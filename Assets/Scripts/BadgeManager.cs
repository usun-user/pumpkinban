using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BadgeManager : MonoBehaviour
{
    // global singleton that any script can access by saying "BadgeManager.Instance.MyMethod()"
    public static BadgeManager Instance;

    [System.Serializable]
    class Badge
    {
        public string badgeName;
        public Sprite badgeSprite;
    }

    [SerializeField] Image badgeIcon;
    [SerializeField] TextMeshProUGUI badgeText;
    [SerializeField] Badge[] badges;
    [SerializeField] Animator badgeAnimator;
    [SerializeField] AudioSource nonpersistentSoundSource;
    [SerializeField] AudioClip badgeSound;

    // stores badges waiting to be displayed
    Queue<int> badgeQueue = new Queue<int>();

    // is a badge currently being displayed?
    bool isShowingBadge = false;

    private void Awake()
    {
        // ensure only one GameManager instance exists
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool WinBadge(int badgeId)
    {
        // don't award badge that was already earned
        if (DataManager.Instance.badgeArr[badgeId])
            return false;

        // save it and add it to the queue
        DataManager.Instance.SaveBadge(badgeId);
        badgeQueue.Enqueue(badgeId);

        // start processing if no badge is currently showing
        if (!isShowingBadge)
        {
            StartCoroutine(ProcessBadgeQueue());
        }

        return true;

        /*
        nonpersistentSoundSource.PlayOneShot(badgeSound);

        Badge newBadge = badges[badgeId];
        badgeIcon.sprite = newBadge.badgeSprite;
        badgeText.text = newBadge.badgeName;
        badgeAnimator.Play("WinBadge");
        return true;
        */
    }

    private IEnumerator ProcessBadgeQueue()
    {
        isShowingBadge = true;

        while (badgeQueue.Count > 0)
        {
            int badgeId = badgeQueue.Dequeue();

            Badge newBadge = badges[badgeId];

            badgeIcon.sprite = newBadge.badgeSprite;
            badgeText.text = newBadge.badgeName;

            nonpersistentSoundSource.PlayOneShot(badgeSound);

            // restart the badge animation from the beginning
            badgeAnimator.Play("WinBadge", 0, 0f);

            // wait one frame so the Animator updates
            yield return null;

            // wait until the animation has finished playing
            while (badgeAnimator.GetCurrentAnimatorStateInfo(0)
                   .normalizedTime < 1f)
            {
                yield return null;
            }
        }

        // allow the next batch of badges to start processing
        isShowingBadge = false;
    }
}

