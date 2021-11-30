using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infinadeck;

/**
 * ------------------------------------------------------------
 * Script to translate Infinadeck motion into game motion.
 * http://tinyurl.com/InfinadeckSDK
 * Created by George Burger & Griffin Brunner @ Infinadeck, 2019
 * Attribution required.
 * ------------------------------------------------------------
 */

public class InfinadeckLocomotion : MonoBehaviour
{
    public GameObject cameraRig;

    [InfReadOnlyInEditor] public float xDistance;
    [InfReadOnlyInEditor] public float yDistance;

    public SpeedVector2 speeds;

    private float fixAngle;
    private float calcX;
    private float calcY;
    public Vector3 worldScale = Vector3.one;
    public float speedGain = 1;
    /**
     * Runs once per frame update.
     */
    void Update () {

        if (Infinadeck.Infinadeck.CheckConnection()) // only run if there is a successful connection
        {
            Debug.Log("good connection");
            // Import speeds from Infinadeck
			speeds = Infinadeck.Infinadeck.GetFloorSpeeds();
            // Distance = speed * time between samples
            //Debug.Log(speeds.v0 + "    " + speeds.v1);
            calcX = (float)speeds.v0 * (Time.deltaTime);
            calcY = (float)speeds.v1 * (Time.deltaTime);
            // Convert for any weird world rotation or scale
            fixAngle = this.transform.eulerAngles.y* Mathf.Deg2Rad;
            xDistance =  (calcX * Mathf.Cos(fixAngle) + calcY * Mathf.Sin(fixAngle)) * worldScale.x * speedGain;
            yDistance = (-calcX * Mathf.Sin(fixAngle) + calcY * Mathf.Cos(fixAngle)) * worldScale.z * speedGain;
            // Move user based on treadmill motion as long as the deck is not calibrating
            if (!Infinadeck.Infinadeck.GetCalibrating()) { cameraRig.transform.position += new Vector3(xDistance, 0, yDistance); }
        }
    }
}