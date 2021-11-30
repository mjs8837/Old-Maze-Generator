using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    GameObject parentObject;

    [SerializeField]
    GameObject mazeGenerator;

    float xPosition;
    float zPosition;

    float angle;

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = Vector3.zero;
        parentObject = gameObject.transform.parent.gameObject;

        angle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        /*xPosition = Mathf.Sin(Mathf.Deg2Rad * angle) * 2.0f;
        zPosition = Mathf.Cos(Mathf.Deg2Rad * angle) * 2.0f;

        angle += 360.0f / 1080.0f;

        if (angle >= 360.0f)
        {
            angle = 0;
        }

        GameObject.Find("ObjectToFollow").transform.position = new Vector3(xPosition + parentObject.transform.position.x, 0.0f, zPosition + parentObject.transform.position.z);*/
    }
}
