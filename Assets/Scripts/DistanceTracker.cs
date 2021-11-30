﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceTracker : MonoBehaviour
{
    [SerializeField] GameObject infinadeck;
    [SerializeField] GameObject startDirection;

    [SerializeField] Camera endPointCamera;

    [SerializeField] AudioSource dingSound;

    List<Vector3> pointList;

    float xDistance;
    float yDistance;

    float totalDisplacement;
    float totalDistance;

    bool isMovement;

    float pointTimer;
    float startTimer;
    float runTimer;

    GameObject startSphere;
    Vector3 cameraForward;

    bool calcDistance;

    // Start is called before the first frame update
    void Start()
    {
        if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/ProjectData/Test.txt"))
        {
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/ProjectData/Test.txt");
        }

        dingSound.enabled = false;
        cameraForward = GameObject.Find("Camera").transform.forward;

        startSphere = Instantiate(startDirection, 15.0f * cameraForward, Quaternion.identity);

        isMovement = false;
        pointList = new List<Vector3>();
        pointTimer = 0.1f;
        startTimer = 5.0f;
        runTimer = 60.0f;
        calcDistance = false;
    }

    // Update is called once per frame
    void Update()
    {
        StartTimer();

        MovementCheck();

        /*if (pointList.Count > 1)
        {
            //Setting the displacement from the starting point
            totalDisplacement = Mathf.Abs(pointList[0].magnitude - pointList[pointList.Count - 1].magnitude);
        }*/

        // Decreasing the timer to create a new point along the users path
        if (isMovement) 
        {
            if (pointTimer > 0.0f)
            {
                pointTimer -= Time.deltaTime;
            }

            //Adding a point based on where the user is and resetting the timer
            else
            {
                if (runTimer > 0.0f)
                {
                    pointList.Add(gameObject.transform.parent.transform.position);
                    HandleTextFile.WriteString(gameObject.transform.parent.transform.position);
                    pointTimer = 0.1f;
                }
            }

            if (runTimer > 0.0f)
            {
                runTimer -= Time.deltaTime;
            }
        }

        //Checking when the runTimer has reached 0 and drawing the users path
        if (runTimer <= 0.0f)
        {
            //Plotting a line along the path the user walks
            for (int i = 0; i < pointList.Count; i++)
            {
                //line.SetPosition(i, pointList[i]);

                if (i + 1 != pointList.Count)
                {
                    Debug.DrawLine(pointList[i], pointList[i + 1], Color.red);
                }
            }

            endPointCamera.transform.position = new Vector3(pointList[pointList.Count - 1].x, 10, pointList[pointList.Count - 1].z);
            runTimer = 0.0f;

            if (!calcDistance)
            {
                for (int i = 0; i < pointList.Count; i++)
                {
                    if (i + 1 != pointList.Count)
                    {
                        totalDistance += Mathf.Abs(pointList[i].magnitude - pointList[i + 1].magnitude);
                    }
                    else
                    {
                        calcDistance = true;
                    }
                }
            }

            isMovement = false;
        }
    }

    private void StartTimer()
    {
        if (startTimer > 0.0f)
        {
            startTimer -= Time.deltaTime;
        }

        else
        {
            dingSound.enabled = true;
            startSphere.SetActive(false);
        }
    }

    private void MovementCheck()
    {
        xDistance = infinadeck.GetComponentInChildren<InfinadeckLocomotion>().xDistance;
        yDistance = infinadeck.GetComponentInChildren<InfinadeckLocomotion>().yDistance;

        //Checking if there is infinadeck movement
        if (Mathf.Abs(xDistance) > 0.00001f || Mathf.Abs(yDistance) > 0.00001f)
        {
            isMovement = true;
        }
        else
        {
            isMovement = false;
        }
    }

    private void OnGUI()
    {
        float seconds = Mathf.Floor(runTimer % 60.0f);
        float minutes = Mathf.Floor(runTimer / 60.0f);

        GUI.Label(new Rect(10, 10, 300, 300), "Green = Start\nRed = End\nTime Remaining: " + string.Format("{0:0}", minutes) +
            ":" + string.Format("{0:00}", seconds));
    }
}
