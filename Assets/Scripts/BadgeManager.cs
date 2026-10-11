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

    public void WinBadge(int badgeId)
    {
        if (DataManager.Instance.badgeArr[badgeId])
            return;

        DataManager.Instance.SaveBadge(badgeId);

        nonpersistentSoundSource.PlayOneShot(badgeSound);

        Badge newBadge = badges[badgeId];
        badgeIcon.sprite = newBadge.badgeSprite;
        badgeText.text = newBadge.badgeName;
        badgeAnimator.Play("WinBadge");
    }
}

