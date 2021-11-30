using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallVisualize : MonoBehaviour
{
    public GameObject wallMaster;
    public GameObject[] ourWalls;
    private MeshRenderer[] ourMeshRenderer;
    public GameObject wallGuide;
    // Update is called once per frame

    void Start()
    {
        wallGuide.GetComponent<MeshRenderer>().enabled = false;
    }
    void Update()
    {
        ShowIt(wallMaster.activeSelf);
    }

    void ShowIt(bool val)
    {
        foreach (GameObject wall in ourWalls)
        {
            wall.GetComponent<MeshRenderer>().enabled = val;
        }
    }
}
