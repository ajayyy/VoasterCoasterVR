﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {

    public RectTransform rectTransform;

    public bool moving;
    //these values will be compared against while the window is moving around
    public Vector3 movingStartPosition;
    public Vector3 movingContollerStartPosition;

    //All of the buttons. To call click on them
    public WindowButton[] buttons;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
	}
	
	void FixedUpdate () {
        GameController gameController = GameController.instance;

        if (moving && gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            transform.position = movingStartPosition + (gameController.rightControllerObject.transform.position - movingContollerStartPosition);

            //rotate it towards the controller
            transform.LookAt(gameController.rightControllerObject.transform);
        } else if (moving && !gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            moving = false;
        }

        //check if this window is being pointed at to see if a button is being pressed or the window needs to be moved
        //only checking right controller for now. TODO: make it so that you can toggle your dominant hand
        if (!moving && gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger) && gameController.rightControllerWindowPointingAt != null) {
            for (int i = 0; i < buttons.Length; i++) {
                if (gameController.rightControllerWindowPointingAt == buttons[i].gameObject) {
                    if (buttons[i].type == 1) {
                        //self destruct
                        Destroy(gameObject);
                    } else {
                        //let the WindowButton class handle the click internally
                        buttons[i].Clicked();
                    }
                    break;
                }
            }

            if (gameController.rightControllerWindowPointingAt == gameObject) {
                //start moving window
                moving = true;
                movingStartPosition = transform.position;
                movingContollerStartPosition = gameController.rightControllerObject.transform.position;
            }
        }
	}
}