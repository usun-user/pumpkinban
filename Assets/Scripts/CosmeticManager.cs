using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticManager : MonoBehaviour
{
    [SerializeField] GameObject hatPanel, facePanel, bodyPanel;
    [SerializeField] Image hat, face, body;
    [SerializeField] Sprite[] hatSprites, faceSprites, bodySprites;
    [SerializeField] Toggle[] hatToggles, faceToggles, bodyToggles;

    public bool hasChangedCosmetics;

    int hatIndex, faceIndex, bodyIndex; // 0 means null sprite (no accessory)
    SpriteRenderer playerHat, playerFace, playerBody;
    Toggle currentHatToggle, currentFaceToggle, currentBodyToggle,
        previousHatToggle, previousFaceToggle, previousBodyToggle;

    void Start()
    {
        playerHat = GameManager.Instance.hat;
        playerFace = GameManager.Instance.face;
        playerBody = GameManager.Instance.body;

        CosmeticSprites cosmetics = DataManager.Instance.GetCosmeticSprites();
        hatIndex = cosmetics.newHatIndex;
        faceIndex = cosmetics.newFaceIndex;
        bodyIndex = cosmetics.newBodyIndex;

        if (hatIndex != 0)
        {
            currentHatToggle = hatToggles[hatIndex];
            currentHatToggle.SetIsOnWithoutNotify(true);
            SelectItem(hat, playerHat, previousHatToggle, hatSprites[hatIndex]);
        }
        if (faceIndex != 0)
        {
            currentFaceToggle = faceToggles[faceIndex];
            currentFaceToggle.SetIsOnWithoutNotify(true);
            SelectItem(face, playerFace, previousFaceToggle, faceSprites[faceIndex]);
        }
        if (bodyIndex != 0)
        {
            currentBodyToggle = bodyToggles[bodyIndex];
            currentBodyToggle.SetIsOnWithoutNotify(true);
            SelectItem(body, playerBody, previousBodyToggle, bodySprites[bodyIndex]);
        }
    }

    public void SetToggle(Toggle newToggle)
    {
        if (hatPanel.activeSelf)
        {
            SetToggleHelper(ref currentHatToggle, ref previousHatToggle, newToggle);
        }
        else if (facePanel.activeSelf)
        {
            SetToggleHelper(ref currentFaceToggle, ref previousFaceToggle, newToggle);
        }
        else if (bodyPanel.activeSelf)
        {
            SetToggleHelper(ref currentBodyToggle, ref previousBodyToggle, newToggle);
        }
    }

    //C# (and Java) passes references by value, so without "ref" the method would only change the local variables inside it
    private void SetToggleHelper(ref Toggle currentToggle, ref Toggle previousToggle, Toggle newToggle)
    {
        if (newToggle != currentToggle)
        {
            previousToggle = currentToggle;
            currentToggle = newToggle;
        }
    }

    public void ChangeCosmeticItem(Sprite newCosmeticItem)
    {
        if (hatPanel.activeSelf)
        {
            if (hat.sprite == newCosmeticItem)
            {
                ClearItem(hat, playerHat, currentHatToggle);
            }
            else
            {
                SelectItem(hat, playerHat, previousHatToggle, newCosmeticItem);
            }
        }
        else if (facePanel.activeSelf)
        {
            if (face.sprite == newCosmeticItem)
            {
                ClearItem(face, playerFace, currentFaceToggle);
            }
            else
            {
                SelectItem(face, playerFace, previousFaceToggle, newCosmeticItem);
            }
        }
        else if (bodyPanel.activeSelf)
        {
            if (body.sprite == newCosmeticItem)
            {
                ClearItem(body, playerBody, currentBodyToggle);
            }
            else
            {
                SelectItem(body, playerBody, previousBodyToggle, newCosmeticItem);
            }
        }
        hasChangedCosmetics = true;
    }

    private void SelectItem(Image cosmeticItem, SpriteRenderer playerCosmeticItem, Toggle previousToggle, Sprite newCosmeticItem)
    {
        cosmeticItem.sprite = newCosmeticItem;
        cosmeticItem.enabled = true;
        playerCosmeticItem.sprite = newCosmeticItem;
        playerCosmeticItem.enabled = true;
        if (previousToggle)
        {
            previousToggle.SetIsOnWithoutNotify(false);
        }
    }

    private void ClearItem(Image cosmeticItem, SpriteRenderer playerCosmeticItem, Toggle currentToggle)
    {
        cosmeticItem.enabled = false;
        cosmeticItem.sprite = null;
        playerCosmeticItem.enabled = false;
        playerCosmeticItem.sprite = null;
        currentToggle.SetIsOnWithoutNotify(false);
    }

    private void ClearAllCosmetics()
    {
        if (currentHatToggle)
        {
            ClearItem(hat, playerHat, currentHatToggle);
        }
        if (currentFaceToggle)
        {
            ClearItem(face, playerFace, currentFaceToggle);
        }
        if (currentBodyToggle)
        {
            ClearItem(body, playerBody, currentBodyToggle);
        }
    }

    public void ClearCosmetics()
    {
        if (hatPanel.activeSelf && currentHatToggle)
        {
            ClearItem(hat, playerHat, currentHatToggle);
        } else if (facePanel.activeSelf && currentFaceToggle)
        {
            ClearItem(face, playerFace, currentFaceToggle);
        }
        else if (bodyPanel.activeSelf && currentBodyToggle)
        {
            ClearItem(body, playerBody, currentBodyToggle);
        } else
        {
            ClearAllCosmetics();
        }
        hasChangedCosmetics = true;
    }

    public void SaveCurrentCosmetics()
    {
        hasChangedCosmetics = false;

        if (hat.sprite == null)
        {
            hatIndex = 0;
        }
        else
        {
            hatIndex = System.Array.IndexOf(hatSprites, hat.sprite);
        }

        if (face.sprite == null)
        {
            faceIndex = 0;
        }
        else
        {
            faceIndex = System.Array.IndexOf(faceSprites, face.sprite);
        }

        if (body.sprite == null)
        {
            bodyIndex = 0;
        }
        else
        {
            bodyIndex = System.Array.IndexOf(bodySprites, body.sprite);
        }

        DataManager.Instance.SaveCosmetics(hatIndex, faceIndex, bodyIndex);
    }
}
