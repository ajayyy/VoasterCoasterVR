﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The preview cart. This will use custom physics to handle drops and turns
public class Cart : MonoBehaviour {

    //the roller coaster this cart is going to be riding
    public RollerCoaster rollerCoaster;

    //velocity in meters
    float velocity = 0;

    //position in meters on the track
    float position = 0;

    //in kg
    float mass = 600;

    bool running = false;

	public void Start () {
        //for now start the position here because there are no chain lifts implemented yet
        position = 7;
        velocity = 0;
    }

    void FixedUpdate () {
        Transform currentBone = GetCurrentBone(true);
        TrackPiece currentTrack = GetCurrentTrack().GetComponent<TrackPiece>();
        Vector3 eulerAnglesOfTrack = currentBone.eulerAngles;
        //find what the incline angle is
        float inclineAngleOfTrack = Mathf.Cos(eulerAnglesOfTrack.y * Mathf.Deg2Rad) * eulerAnglesOfTrack.x + Mathf.Sin(eulerAnglesOfTrack.y * Mathf.Deg2Rad + Mathf.PI) * eulerAnglesOfTrack.z;

        //calculate the force downward (divided by 60 fps)
        float forceDown = (-9.81f) / 60f;

        //calculate the amount of that force used on an incline of the angle (same as acceleration)
        float gravityAcceleration = Mathf.Sin(inclineAngleOfTrack * Mathf.Deg2Rad) * forceDown;

        //calculate the new movements
        velocity += gravityAcceleration;

        //check if this track is has a chain lift
        if (currentTrack.chainLift && velocity < currentTrack.chainSpeed) {
            velocity = currentTrack.chainSpeed;
        }

        position += velocity / 60f;

        Transform finalBone = GetCurrentBone(true);
        Transform nextBone = GetNextBone(true, 1);
        float boneNum = GetBoneNum();

        float distanceToNextBone = Vector3.Distance(nextBone.position, finalBone.position);

        //find offset amount needed to center the cart and place cart on top of track
        Vector3 offsetAmount = (new Vector3(1, 0, 0) * rollerCoaster.trackWidth + new Vector3(0, 1, 0) * 2.57f) * GameController.instance.scale;

        Vector3 extraAmount = new Vector3(0, 0, 1) * (distanceToNextBone * (boneNum - (int)boneNum));

        transform.position = finalBone.position + MathHelper.RotatePointAroundPivot(offsetAmount, Vector3.zero, finalBone.rotation) - MathHelper.RotatePointAroundPivot(extraAmount, Vector3.zero, finalBone.rotation);

        Vector3 angleDifference = nextBone.eulerAngles - finalBone.eulerAngles;

        Vector3 extraRotation = angleDifference * (boneNum - (int)boneNum);

        transform.eulerAngles = finalBone.eulerAngles + extraRotation;
    }

    Transform GetCurrentBone(bool right) {
        float boneNum = position / rollerCoaster.defaultTrackBoneSize;
        float trackNum = boneNum / rollerCoaster.boneAmount;

        List<Transform> bones = new List<Transform>();

        //find all the bones for this track piece
        foreach (Transform child in rollerCoaster.trackPieces[(int) trackNum].GetComponentsInChildren<Transform>()) {
            //if right, check under right rail, otherwise check under left rail
            if (((right && child.parent.gameObject.name == "Right_Rail") || (!right && child.parent.gameObject.name == "Left_Rail")) || (bones.Count > 0 && child.parent == bones[bones.Count - 1])) {
                //second if statement as the above one is way too long
                //if the object is disabled, then it does not matter
                //don't include the last joint as that does not matter
                if (child.gameObject.activeInHierarchy && child.gameObject.name != "Joint_10_2" && child.gameObject.name != "Joint_10") {
                    bones.Add(child);
                }
            }
        }

        return bones[(int) boneNum - ((int)trackNum * 10)];
    }

    Transform GetNextBone(bool right, int amount) {
        float boneNum = position / rollerCoaster.defaultTrackBoneSize + amount;
        float trackNum = boneNum / rollerCoaster.boneAmount;

        List<Transform> bones = new List<Transform>();

        //find all the bones for this track piece
        foreach (Transform child in rollerCoaster.trackPieces[(int)trackNum].GetComponentsInChildren<Transform>()) {
            //if right, check under right rail, otherwise check under left rail
            if (((right && child.parent.gameObject.name == "Right_Rail") || (!right && child.parent.gameObject.name == "Left_Rail")) || (bones.Count > 0 && child.parent == bones[bones.Count - 1])) {
                //second if statement as the above one is way too long
                //if the object is disabled, then it does not matter
                //don't include the last joint as that does not matter
                if (child.gameObject.activeInHierarchy && child.gameObject.name != "Joint_10_2" && child.gameObject.name != "Joint_10") {
                    bones.Add(child);
                }
            }
        }

        return bones[(int)boneNum - ((int)trackNum * 10)];
    }

    GameObject GetCurrentTrack() {
        float boneNum = position / rollerCoaster.defaultTrackBoneSize;
        float trackNum = boneNum / rollerCoaster.boneAmount;

        return rollerCoaster.trackPieces[(int)trackNum];
    }

    //finds closest track num
    float GetBoneNum() {
        float boneNum = position / rollerCoaster.defaultTrackBoneSize;

        return boneNum;
    }
}