using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DistanceTracker : MonoBehaviour
{
    [SerializeField] string personName;

    //Connecting the infinadeck and the Unity game object
    [SerializeField] GameObject infinadeck;

    // Serialized field for the prefab of the object placed in front of the user when the pre/post maze starts 
    [SerializeField] GameObject startDirection;

    // Serialized field for the camera that is placed at the end of the users path in pre/post maze
    [SerializeField] Camera endPointCamera;

    // Serialized field that holds ding sound. The file sound is located under Asset/Sounds within the project. The sound will play when the user is allowed to move in the pre/post maze.
    [SerializeField] AudioSource dingSound;

    // List all the point plotted along the user's path in the post maze analysis. A point is added to the list every time pointTimer resets.
    [SerializeField] List<Vector3> pointList;

    // Calculate every frame based on infinadeck motion. Used to check if the user is moving 
    float xDistance;
    float yDistance;

     // Bool that is set based on whether the xDistance and yDistance variables are greater than a certain value.
    bool isMovement;

    // Float set to the amount of time between each plotted point along the users path in the pre/post maze analysis
    float pointTimer;

    // Float that is set to the amount of time before the user is told to move at the begenning of the pre/post maze. It also removes the object in front of the user that they try to reach.
    float startTimer;

    // Float set to the amount of time the user be walking in the pre/post maze analysis.
    [SerializeField] float runTimer;


    [SerializeField] float distanceTraveled;
    float distanceGoal;

    // GameObject that is set in the pre/post maze analysis using the startDirection prefab.
    GameObject startSphere;
    //set to the foward direction the user is facing when they load into the pre/post maze analysis. This is the direction the startSphere will spawn in.
    Vector3 cameraForward;

    [SerializeField] bool runTimerEnabled;

    // Start is called before the first frame update
    // Built in Unity method that runs at the start of the program. It deletes the existing data file on the desktop. It will get the camera's foward vector, spawn in the start sphere, and set all the timers to their respective values.
    void Start()
    {
        runTimerEnabled = false;
        dingSound.enabled = false;
        cameraForward = GameObject.Find("Camera").transform.forward;
        cameraForward.y = cameraForward.y + 0.1f;

        startSphere = Instantiate(startDirection, 15.0f * cameraForward, Quaternion.identity);

        isMovement = false;
        pointList = new List<Vector3>();
        pointTimer = 0.1f;
        startTimer = 5.0f;
        runTimer = 20.0f;
        distanceGoal = 20.0f;
    }

    // Update is called once per frame
    // Build in Unity method that runs once Start is completed after every frame. It will call the StartTimer and MoivementCheck methods along with decreasing the runTimer and plotting the point along the users path. These points will be written into a text document to be plotted later.
    void Update()
    {
        StartTimer();

        MovementCheck();

        if (distanceTraveled <= 0.0001f && runTimer == 10.0f)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                runTimerEnabled = !runTimerEnabled;
            }
        }

        //THIS SECTION USES TIME AS A VARIABLE

        if (runTimerEnabled)
        {
            // Decreasing the timer to create a new point along the users path
            if (startSphere == null)
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
                        HandleTextFile.WriteString(gameObject.transform.parent.transform.position, personName);
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
                if (SceneManager.GetActiveScene().name == "PreMazeTest")
                {
                    if (!isMovement)
                    {
                        SceneManager.LoadScene("MazeWalker");
                    }
                }

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
            }
        }

        else
        {
            //THIS SECTION USES DISTANCE AS A VARIABLE
            if (startSphere == null)
            {
                if (pointTimer > 0.0f)
                {
                    pointTimer -= Time.deltaTime;
                }

                //Adding a point based on where the user is and resetting the timer
                else
                {
                    if (distanceTraveled < distanceGoal)
                    {
                        pointList.Add(gameObject.transform.parent.transform.position);
                        HandleTextFile.WriteString(gameObject.transform.parent.transform.position, personName);
                        pointTimer = 0.1f;
                    }
                }

                if (distanceTraveled < distanceGoal)
                {
                    distanceTraveled += Mathf.Abs(xDistance) + Mathf.Abs(yDistance);
                }
            }

            //Checking when the runTimer has reached 0 and drawing the users path
            if (distanceTraveled > distanceGoal)
            {
                if (SceneManager.GetActiveScene().name == "PreMazeTest")
                {
                    if (!isMovement)
                    {
                        SceneManager.LoadScene("MazeWalker");
                    }
                }

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
            }
        }
    }
    //Check if the startTimer has reached zero and enable the ding sound to tell the user to move and remove the sphere in the direction they were originally facing.
    private void StartTimer()
    {
        if (startTimer > 0.0f)
        {
            startTimer -= Time.deltaTime;
        }

        else
        {
            dingSound.enabled = true;

            if (startSphere != null)
            {
                startSphere.SetActive(false);
                Destroy(startSphere);
            }
        }
    }
    // Check if there is noticeable Infinadeck motion. This checks against the xDistance and yDistance variables
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
        if (runTimerEnabled)
        {
            float seconds = Mathf.Floor(runTimer % 60.0f);
            float minutes = Mathf.Floor(runTimer / 60.0f);

            GUI.Label(new Rect(10, 10, 300, 300), "Green = Start\nRed = End\nTime Remaining: " + string.Format("{0:0}", minutes) +
                ":" + string.Format("{0:00}", seconds));
        }
        else
        {
            float distanceRemaining = distanceGoal - distanceTraveled;
            GUI.Label(new Rect(10, 10, 300, 300), "Green = Start\nRed = End\nDistance Remaining: " + string.Format("{0:00}", distanceRemaining));
        }
    }
}
