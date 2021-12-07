using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Infinadeck;


public class MazeGenerator : MonoBehaviour
{
    public int[,] m;
    public int[,] mirror;
    public int sizeOfMaze = 32;
    public float angleThreshold = .5f;
    public float openThreshold = .3f;
    public float hallsize = 2;
    public float wallheight = 4;
    public float updateTime = .001f;
    public int delayedUpdate = 50;
    public bool deferredRendering = true;
    public float overviewHeight = 10;

    public bool mazeFinished = false;
    public bool isMovement = false;

    [SerializeField] GameObject infinadeck;
    [SerializeField] GameObject cameraRig;
    [SerializeField] AudioSource dingAudio;
    [SerializeField] GameObject objective;

    GameObject newObjective;

    float xDistance;
    float yDistance;
    [SerializeField] float rotationFactor;

    float runTimer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BuildMaze());

        //Moving the infinadeck reference in scene out of sight
        GameObject temp = GameObject.Find("InfinadeckReferenceObjects(Clone)");
        temp.transform.position += new Vector3(0.0f, -1.0f, 0.0f);

        rotationFactor = 5.0f;

        float randomNum = Random.Range(0.0f, 2.0f);

        if (randomNum < 1.0f)
        {
            rotationFactor = -rotationFactor;
        }

        runTimer = 3.0f;

        //Creating an "objective" for the user to go towards
        newObjective = Instantiate(objective, new Vector3(Random.Range(0.0f, 18.0f), -10.0f, Random.Range(0.0f, 18.0f)), Quaternion.identity);
        newObjective.transform.SetParent(transform);

        dingAudio.enabled = false;
    }
    private void Update()
    {
        MazeControls();

        MovementCheck();

        CheckObjectiveDistance();

        TimerChecks();

        MazeRotation();
    }
    IEnumerator BuildMaze()
    {
        transform.localPosition = new Vector3(0, -overviewHeight, 0);
        int loop = 0;
        int loopLimit = 50;
        sizeOfMaze = 2 * ((sizeOfMaze + 1) / 2); //rounding maze size up to smallest useable
        m = new int[sizeOfMaze, sizeOfMaze];
        mirror = new int[sizeOfMaze, sizeOfMaze];
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
            loop++;
            if (loop > loopLimit)
            {
                loop = 0;
                yield return new WaitForSeconds(updateTime);
            }
        }
        transform.localScale = Vector3.one;
        // REFRESH ENTIRE BLOCK
        for (int i = 0; i < sizeOfMaze; i++)
        {
            for (int j = 0; j < sizeOfMaze; j++)
            {
                m[i, j] = 0;
                mirror[i, j] = 0;
                loop++;
                if (loop > loopLimit)
                {
                    loop = 0;
                    yield return new WaitForSeconds(updateTime);
                }
            }
        }

        // INIT MAZE- CLEAR ANY EXISTING SETTINGS, PLACE KNOWN SOLIDS
        for (int i = 0; i < sizeOfMaze; i++)
        {
            for (int j = 0; j < sizeOfMaze; j++)
            {
                if ((i % 2 == 0) && (j % 2 == 0))
                {
                    //both even, leave empty
                    m[i, j] = 0;
                }
                else if ((i % 2 != 0) && (j % 2 != 0))
                {
                    //both odd, set as transient
                    m[i, j] = 2;//////////////////////////////////////////////////////////////////////////////////////////////TEMPORARY
                }

                if (i == sizeOfMaze - 1 || j == sizeOfMaze - 1)
                {
                    //far walls
                    m[i, j] = 1;
                }
                loop++;
                if (loop > loopLimit)
                {
                    loop = 0;
                    yield return new WaitForSeconds(updateTime);
                }

            }
        }
        loopLimit = 20;
        // SPAWN RANDOMIZED MISSING WALLS
        for (int i = 0; i < sizeOfMaze; i++)
        {
            for (int j = 0; j < sizeOfMaze; j++)
            {
                if ((i % 2 == 0) && (j % 2 == 0))// && i == 0 && j == 0)
                {
                    Debug.Log("FOR " + i + "," + j + ":");
                    //both even
                    int[] iPossibleOptions = new int[sizeOfMaze / 2 - 1];
                    int[] jPossibleOptions = new int[sizeOfMaze / 2 - 1];
                    bool undefined = true;
                    int superT = 1;
                    while (undefined)
                    {
                        //Debug.Log(i);
                        //Debug.Log(j);
                        //Debug.Log(m[i, j]);
                        //Debug.Log(m[i+1, j]);
                        //Debug.Log(m[i, j+1]);
                        //Debug.Log(undefined);
                        if (m[i + 1, j] != 0 || m[i, j + 1] != 0)
                        {

                            Debug.Log(i + "," + j + " all in");
                            undefined = false;
                        }
                        else
                        {
                            Debug.Log("checking " + i + "," + j + " " + superT + "time!");
                            superT++;
                            //initialize limits: Min is actual walking space. 
                            int iMin = i;
                            int iMax = iMin;
                            int jMin = j;
                            int jMax = jMin;
                            //find far x wall
                            for (int k = 1; (2 * k) + i + 1 <= sizeOfMaze; k++)
                            {
                                if (m[(2 * k) + i + 1, j] != 0)
                                {
                                    iMax = (2 * k) + i;
                                    break;
                                }
                            }
                            //find far y wall
                            for (int k = 0; (2 * k) + j + 1 <= sizeOfMaze; k++)
                            {
                                if (m[i, (2 * k) + j + 1] != 0)
                                {
                                    jMax = (2 * k) + j;
                                    break;
                                }
                            }
                            int iMinWallOption = iMin + 1;
                            int iMinExistingWall = iMin - 1;
                            int iMaxWallOption = iMax - 1;
                            int iMaxExistingWall = iMax + 1;
                            int jMinWallOption = jMin + 1;
                            int jMinExistingWall = jMin - 1;
                            int jMaxWallOption = jMax - 1;
                            int jMaxExistingWall = jMax + 1;
                            int iT = 0;
                            for (int ii = iMinWallOption; ii <= iMaxWallOption; ii += 2)
                            {
                                iPossibleOptions[iT] = ii;
                                iT++;
                            }
                            int jT = 0;
                            for (int jj = jMinWallOption; jj <= jMaxWallOption; jj += 2)
                            {
                                jPossibleOptions[jT] = jj;
                                jT++;
                            }
                            iT = Mathf.FloorToInt(Random.value * (iT));
                            jT = Mathf.FloorToInt(Random.value * (jT));
                            int iWall = iPossibleOptions[iT];
                            int jWall = jPossibleOptions[jT];
                            Debug.Log("Current Wonderwalls: " + iWall + " " + jWall);
                            //find wall subdivisions
                            for (int k = 0; (2 * k) + j + 1 <= sizeOfMaze; k++)
                            {
                                if (m[i, (2 * k) + j + 1] != 0)
                                {
                                    jMax = (2 * k) + j;
                                    break;
                                }
                            }
                            int[] iPossibleLowOptions = new int[sizeOfMaze / 2];
                            int[] iPossibleHighOptions = new int[sizeOfMaze / 2];
                            int[] jPossibleLowOptions = new int[sizeOfMaze / 2];
                            int[] jPossibleHighOptions = new int[sizeOfMaze / 2];
                            for (int k = 0; k < sizeOfMaze / 2; k++)
                            {
                                iPossibleLowOptions[k] = -1;
                                iPossibleHighOptions[k] = -1;
                                jPossibleLowOptions[k] = -1;
                                jPossibleHighOptions[k] = -1;
                            }
                            int iLT = 0;
                            for (int ii = iMin; ii <= iWall - 1; ii += 2)
                            {
                                iPossibleLowOptions[iLT] = ii;
                                iLT++;
                            }
                            int iHT = 0;
                            for (int ii = iWall + 1; ii <= iMax; ii += 2)
                            {
                                iPossibleHighOptions[iHT] = ii;
                                iHT++;
                            }
                            int jLT = 0;
                            for (int jj = jMin; jj <= jWall - 1; jj += 2)
                            {
                                jPossibleLowOptions[jLT] = jj;
                                jLT++;
                            }
                            int jHT = 0;
                            for (int jj = jWall + 1; jj <= jMax; jj += 2)
                            {
                                jPossibleHighOptions[jHT] = jj;
                                jHT++;
                            }
                            int disclude = Mathf.CeilToInt(Random.value * 4);
                            if (disclude != 1)
                            {
                                int pos = Mathf.FloorToInt(Random.value * (iLT));
                                //iPossibleLowOptions[pos] = -1;
                                mirror[iPossibleLowOptions[pos], jWall] = 1;
                                Debug.Log("For xlow, defer " + iPossibleLowOptions[pos] + "," + jWall);
                            }
                            if (disclude != 2)
                            {
                                int pos = Mathf.FloorToInt(Random.value * (iHT));
                                mirror[iPossibleHighOptions[pos], jWall] = 1;
                                Debug.Log("For xhigh, defer " + iPossibleHighOptions[pos] + "," + jWall);
                            }
                            if (disclude != 3)
                            {
                                int pos = Mathf.FloorToInt(Random.value * (jLT));
                                mirror[iWall, jPossibleLowOptions[pos]] = 1;
                                Debug.Log("For ylow, defer " + iWall + "," + jPossibleLowOptions[pos]);
                            }
                            if (disclude != 4)
                            {
                                int pos = Mathf.FloorToInt(Random.value * (jHT));
                                mirror[iWall, jPossibleHighOptions[pos]] = 1;
                                Debug.Log("For yhigh, defer " + iWall + "," + jPossibleHighOptions[pos]);
                            }
                            //Debug.Log(iPossibleLowOptions[0] + " " + iPossibleLowOptions[1] + " " + iPossibleLowOptions[2] + " " + iPossibleLowOptions[3] + " " + iPossibleLowOptions[4]
                            //    + " " + iPossibleLowOptions[5] + " " + iPossibleLowOptions[6] + " " + iPossibleLowOptions[7] + " " + iPossibleLowOptions[8] + " " + iPossibleLowOptions[9]
                            //    + " " + iPossibleLowOptions[10] + " " + iPossibleLowOptions[11] + " " + iPossibleLowOptions[12] + " " + iPossibleLowOptions[13] + " " + iPossibleLowOptions[14]);
                            //Debug.Log(iPossibleHighOptions[0] + " " + iPossibleHighOptions[1] + " " + iPossibleHighOptions[2] + " " + iPossibleHighOptions[3] + " " + iPossibleHighOptions[4]
                            //    + " " + iPossibleHighOptions[5] + " " + iPossibleHighOptions[6] + " " + iPossibleHighOptions[7] + " " + iPossibleHighOptions[8] + " " + iPossibleHighOptions[9]
                            //    + " " + iPossibleHighOptions[10] + " " + iPossibleHighOptions[11] + " " + iPossibleHighOptions[12] + " " + iPossibleHighOptions[13] + " " + iPossibleHighOptions[14]);
                            //Debug.Log(jPossibleLowOptions[0] + " " + jPossibleLowOptions[1] + " " + jPossibleLowOptions[2] + " " + jPossibleLowOptions[3] + " " + jPossibleLowOptions[4]
                            //    + " " + jPossibleLowOptions[5] + " " + jPossibleLowOptions[6] + " " + jPossibleLowOptions[7] + " " + jPossibleLowOptions[8] + " " + jPossibleLowOptions[9]
                            //    + " " + jPossibleLowOptions[10] + " " + jPossibleLowOptions[11] + " " + jPossibleLowOptions[12] + " " + jPossibleLowOptions[13] + " " + jPossibleLowOptions[14]);
                            //Debug.Log(jPossibleHighOptions[0] + " " + jPossibleHighOptions[1] + " " + jPossibleHighOptions[2] + " " + jPossibleHighOptions[3] + " " + jPossibleHighOptions[4]
                            //    + " " + jPossibleHighOptions[5] + " " + jPossibleHighOptions[6] + " " + jPossibleHighOptions[7] + " " + jPossibleHighOptions[8] + " " + jPossibleHighOptions[9]
                            //    + " " + jPossibleHighOptions[10] + " " + jPossibleHighOptions[11] + " " + jPossibleHighOptions[12] + " " + jPossibleHighOptions[13] + " " + jPossibleHighOptions[14]);
                            foreach (int ii in iPossibleLowOptions)
                            {
                                if (ii != -1) { m[ii, jWall] = 1; }
                            }
                            foreach (int ii in iPossibleHighOptions)
                            {
                                if (ii != -1) { m[ii, jWall] = 1; }
                            }
                            foreach (int jj in jPossibleLowOptions)
                            {
                                if (jj != -1) { m[iWall, jj] = 1; }
                            }
                            foreach (int jj in jPossibleHighOptions)
                            {
                                if (jj != -1) { m[iWall, jj] = 1; }
                            }
                        }
                        loop++;
                        if (loop > loopLimit)
                        {
                            loop = 0;
                            yield return new WaitForSeconds(updateTime);
                        }
                        Debug.Log("Calculated some Walls for " + i + " " + j);
                    }
                }
                if (sizeOfMaze > delayedUpdate || deferredRendering) { yield return new WaitForSeconds(updateTime); }
            }
            if (deferredRendering) { yield return new WaitForSeconds(updateTime); }
        }
        if (deferredRendering) { yield return new WaitForSeconds(updateTime); }

        //CORRECT GATEWAYS, LOGGED AS 9 THUS FAR
        int Tnew = 0;
        for (int i = 0; i < sizeOfMaze; i++)
        {
            for (int j = 0; j < sizeOfMaze; j++)
            {
                if (Random.value < openThreshold && m[i, j] != 2 && i < sizeOfMaze - 1 && j < sizeOfMaze - 1)
                {
                    m[i, j] = 0; Tnew++;
                }
                if (mirror[i, j] == 1) { m[i, j] = 0; Tnew++; }
                Debug.Log("Propagated Pocket Check at " + i + " " + j);
                if (sizeOfMaze > delayedUpdate || deferredRendering) { yield return new WaitForSeconds(updateTime); }
                loop++;
                if (loop > loopLimit)
                {
                    loop = 0;
                    yield return new WaitForSeconds(updateTime);
                }
            }
            if (deferredRendering) { yield return new WaitForSeconds(updateTime); }
        }
        Debug.Log("Total Tnew: " + Tnew);
        if (deferredRendering) { yield return new WaitForSeconds(updateTime); }
        loopLimit = 10;
        //DETERMINE TRANSIENT RESULTS
        for (int i = 0; i < sizeOfMaze - 1; i++)
        {
            for (int j = 0; j < sizeOfMaze - 1; j++)
            {
                if (m[i, j] == 2) //implies transient
                {
                    bool north = false;
                    bool east = false;
                    bool south = false;
                    bool west = false;
                    if (m[i, j + 1] == 1) { north = true; }
                    if (m[i + 1, j] == 1) { east = true; }
                    if (m[i, j - 1] == 1) { south = true; }
                    if (m[i - 1, j] == 1) { west = true; }
                    if (Random.value > angleThreshold)
                    {
                        if (!north && !east && !south && !west)
                        {
                            m[i, j] = 0;
                        }
                        else { m[i, j] = 1; }
                    }
                    else
                    {

                        if (north)
                        {
                            if (east)
                            {
                                if (south)
                                {
                                    if (west)
                                    {
                                        //set 1 for solid
                                        m[i, j] = 1;
                                    }
                                    else
                                    {
                                        //set 12 for nes
                                        m[i, j] = 12;
                                    }
                                }
                                else
                                {
                                    if (west)
                                    {
                                        //set 15 for enw
                                        m[i, j] = 15;
                                    }
                                    else
                                    {
                                        //set 6 for ne
                                        m[i, j] = 6;
                                    }
                                }
                            }
                            else
                            {
                                if (south)
                                {
                                    if (west)
                                    {
                                        //set 14 for nws
                                        m[i, j] = 14;
                                    }
                                    else
                                    {
                                        //set 10 for ns
                                        m[i, j] = 10;
                                    }
                                }
                                else
                                {
                                    if (west)
                                    {
                                        //set 9 for nw
                                        m[i, j] = 9;
                                    }
                                    else
                                    {
                                        //set 2 for n
                                        m[i, j] = 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (east)
                            {
                                if (south)
                                {
                                    if (west)
                                    {
                                        //set 13 for esw
                                        m[i, j] = 13;
                                    }
                                    else
                                    {
                                        //set 7 for se
                                        m[i, j] = 7;
                                    }
                                }
                                else
                                {
                                    if (west)
                                    {
                                        //set 11 for ew
                                        m[i, j] = 11;
                                    }
                                    else
                                    {
                                        //set 3 for e
                                        m[i, j] = 3;
                                    }
                                }
                            }
                            else
                            {
                                if (south)
                                {
                                    if (west)
                                    {
                                        //set 8 for sw
                                        m[i, j] = 8;
                                    }
                                    else
                                    {
                                        //set 4 for s
                                        m[i, j] = 4;
                                    }
                                }
                                else
                                {
                                    if (west)
                                    {
                                        //set 5 for w
                                        m[i, j] = 5;
                                    }
                                    else
                                    {
                                        //set 0 for empty
                                        m[i, j] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
                loop++;
                if (loop > loopLimit)
                {
                    loop = 0;
                    yield return new WaitForSeconds(updateTime);
                }
                Debug.Log("Calculated Advanced Geometry for " + i + " " + j);
                if (sizeOfMaze > delayedUpdate || deferredRendering) { yield return new WaitForSeconds(updateTime); }
            }
            //set end coordinate and spawn coordinate offset by +-16,+-16
            if (deferredRendering) { yield return new WaitForSeconds(updateTime); }
        }
        if (deferredRendering) { yield return new WaitForSeconds(updateTime); }
        loopLimit = 5;
        GameObject wall;
        for (int i = -1; i < sizeOfMaze; i++)
        {
            wall = Instantiate(Resources.Load("Prefabs/" + "1- Solid") as GameObject, new Vector3(i, -overviewHeight, -1), Quaternion.identity);
            wall.transform.parent = transform;
            wall.name = "Wall " + i + "," + -1 + " [" + wall.name + "]";
            loop++;
            if (loop > loopLimit)
            {
                loop = 0;
                yield return new WaitForSeconds(updateTime);
            }
        }
        for (int j = 0; j < sizeOfMaze; j++)
        {
            wall = Instantiate(Resources.Load("Prefabs/" + "1- Solid") as GameObject, new Vector3(-1, -overviewHeight, j), Quaternion.identity);
            wall.transform.parent = transform;
            wall.name = "Wall " + -1 + "," + j + " [" + wall.name + "]";
            loop++;
            if (loop > loopLimit)
            {
                loop = 0;
                yield return new WaitForSeconds(updateTime);
            }
        }
        string writeString = "0- Empty";
        for (int i = 0; i < sizeOfMaze; i++)
        {
            for (int j = 0; j < sizeOfMaze; j++)
            {
                if (m[i, j] == 0)
                {
                    writeString = "0- Empty";
                }
                else if (m[i, j] == 1)
                {
                    writeString = "1- Solid";
                }
                else if (m[i, j] == 2)
                {
                    writeString = "2- N Angle";
                }
                else if (m[i, j] == 3)
                {
                    writeString = "3- E Angle";
                }
                else if (m[i, j] == 4)
                {
                    writeString = "4- S Angle";
                }
                else if (m[i, j] == 5)
                {
                    writeString = "5- W Angle";
                }
                else if (m[i, j] == 6)
                {
                    writeString = "6- NE Diag";
                }
                else if (m[i, j] == 7)
                {
                    writeString = "7- SE Diag";
                }
                else if (m[i, j] == 8)
                {
                    writeString = "8- SW Diag";
                }
                else if (m[i, j] == 9)
                {
                    writeString = "9- NW Diag";
                }
                else if (m[i, j] == 10)
                {
                    writeString = "10- NS Relief";
                }
                else if (m[i, j] == 11)
                {
                    writeString = "11- EW Relief";
                }
                else if (m[i, j] == 12)
                {
                    writeString = "12- NES Pocket";
                }
                else if (m[i, j] == 13)
                {
                    writeString = "13- ESW Pocket";
                }
                else if (m[i, j] == 14)
                {
                    writeString = "14- NWS Pocket";
                }
                else if (m[i, j] == 15)
                {
                    writeString = "15- ENW Pocket";
                }
                //Debug.Log("Prefabs/" + writeString);
                wall = Instantiate(Resources.Load("Prefabs/" + writeString) as GameObject, new Vector3(i, -overviewHeight, j), Quaternion.identity);
                wall.transform.parent = transform;
                wall.name = "Wall " + i + "," + j + " [" + wall.name + "]";

                if (i == sizeOfMaze - 1 && j == sizeOfMaze - 1)
                {
                    mazeFinished = true;
                }

                Debug.Log("Spawned Appropriate Wallage at " + i + " " + j);
                if (sizeOfMaze > delayedUpdate || deferredRendering) { yield return new WaitForSeconds(updateTime); }
                loop++;
                if (loop > loopLimit)
                {
                    loop = 0;
                    yield return new WaitForSeconds(updateTime);
                }
            }
            yield return new WaitForSeconds(updateTime);
        }
        yield return new WaitForSeconds(3);
        transform.localScale = new Vector3(hallsize, wallheight, hallsize);
        transform.localPosition = new Vector3(-sizeOfMaze / 2 * hallsize, 0, -sizeOfMaze / 2 * hallsize);
        //check for axis flip as random
        //check for rotation as random  
    }

    //Rotating the maze based on Infinadeck motion
    private void MazeRotation()
    {
        if (infinadeck != null)
        {
            xDistance = infinadeck.GetComponentInChildren<InfinadeckLocomotion>().xDistance;
            yDistance = infinadeck.GetComponentInChildren<InfinadeckLocomotion>().yDistance;
        }

        if (mazeFinished)
        {
            Vector3 angleChange = new Vector3(0.0f, 0.25f, 0.0f) * ((Mathf.Abs(xDistance) + Mathf.Abs(yDistance)) * rotationFactor);

            //Checking whether to rotate clockwise or counter-clockwise
            if (rotationFactor <= 0.0f)
            {
                gameObject.transform.RotateAround(cameraRig.transform.position, Vector3.up, -angleChange.magnitude);
            }
            else
            {
                gameObject.transform.RotateAround(cameraRig.transform.position, Vector3.up, angleChange.magnitude);
            }
        }
    }

    //Maze related controls
    private void MazeControls()
    {
        //Maze related key binds
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(BuildMaze());
        }
    }

    //Checking if the Infinadeck is producing movement
    private void MovementCheck()
    {
        if (Mathf.Abs(xDistance) > 0.00001f || Mathf.Abs(yDistance) > 0.00001f)
        {
            isMovement = true;
        }
        else
        {
            isMovement = false;
        }
    }

    private void CheckObjectiveDistance()
    {
        float objectiveDistance = (newObjective.transform.position - cameraRig.transform.position).magnitude;

        if (objectiveDistance <= 5.0f)
        {
            if (newObjective != null)
            {
                Destroy(newObjective);
                newObjective = Instantiate(objective, new Vector3(Random.Range(-18.0f, 18.0f), 0.0f, Random.Range(-18.0f, 18.0f)), Quaternion.identity);
                newObjective.transform.SetParent(transform);
            }
        }
    }

    private void TimerChecks()
    {
        if (isMovement)
        {
            if (runTimer > 0.0f && mazeFinished)
            {
                runTimer -= Time.deltaTime;
            }
        }

        if (runTimer <= 0.5f && mazeFinished)
        {
            dingAudio.enabled = true;

            if (!isMovement)
            {
                SceneManager.LoadScene("PostMazeTest");
            }
        }
    }

    private void OnGUI()
    {
        float seconds = Mathf.Floor(runTimer % 60.0f);
        float minutes = Mathf.Floor(runTimer / 60.0f); 
        GUI.Label(new Rect(10, 10, 100, 100), "Time Remaining: " + string.Format("{0:0}", minutes) + ":" + string.Format("{0:00}", seconds));
    }
}
