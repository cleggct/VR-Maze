using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ControllerInput : MonoBehaviour {

    private Hand hand; //hand object
    private SteamVR_Controller.Device controller; //the hand's controller
    private ControllerHoverHighlight highlighter;
    //private ulong menuButton = SteamVR_Controller.ButtonMask.ApplicationMenu;

	// Use this for initialization
	void Start () {
        hand = gameObject.GetComponent<Hand>();
        controller = hand.controller;
        highlighter = gameObject.GetComponentInChildren<ControllerHoverHighlight>();
	}
	
	// Update is called once per frame
	void Update () {
        if (controller != null)
        {
            //Debug.Log("Controller not null!");
            if (controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                if (GameManager.paused)
                {
                    ControllerButtonHints.HideButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger); //hide button hint when closing menu
                }
                GameManager.menuPressed = true;
                highlighter.HideHighlight();
            }
        }
        else
        {
            hand = gameObject.GetComponent<Hand>();
            controller = hand.controller;
            highlighter = gameObject.GetComponentInChildren<ControllerHoverHighlight>();
        }
    }

    public void hideControllerHighlight()
    {
        if(highlighter != null)
        {
            highlighter.HideHighlight();
        }
    }

}
