using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.EventSystems;

public class MobileButtonManager : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] GameObject player;
    PlayerManager playerScript;
    [SerializeField] float thisHorizontalInput, thisVerticalInput;

    void Start()
    {
        playerScript = player.GetComponent<PlayerManager>();
    }

    //public void OnPointerDown(PointerEventData eventData)
    public void Down()
    {
        playerScript.mobileHorizontalInput = thisHorizontalInput;
        playerScript.mobileVerticalInput = thisVerticalInput;
    }

    //public void OnPointerUp(PointerEventData eventData)
    public void Up()
    {
        playerScript.mobileHorizontalInput = 0f;
        playerScript.mobileVerticalInput = 0f;
    }
}
