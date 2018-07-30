﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public SteamVR_TrackedObject leftControllerTrackedObject;
    public SteamVR_TrackedObject rightControllerTrackedObject;

    public SteamVR_Controller.Device leftController {
        get {
            return SteamVR_Controller.Input((int)leftControllerTrackedObject.index);
        }
    }
    public SteamVR_Controller.Device rightController {
        get {
            return SteamVR_Controller.Input((int)rightControllerTrackedObject.index);
        }
    }

    //game objects
    public GameObject leftControllerObject;
    public GameObject rightControllerObject;

    public GameObject world;

    //the scale the world is set at
    //the world's scale can change, but by default is 0.008
    public float scale {
        get {
            return world.transform.localScale.x;
        } set {
            world.transform.localScale = new Vector3(value, value, value);
        }
    }

    //the scale from the last frame. Used for adjusting physics based on the scale
    public float lastScale = 1;

    void Start () {
		if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}
	
	void FixedUpdate () {
        if (instance == null) {
            instance = this;
        }

        if(scale != lastScale) {
            Physics.gravity = new Vector3(0, (-9.81f) * scale, 0);
            lastScale = scale;
        }
    }
}
