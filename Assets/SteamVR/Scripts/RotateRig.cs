﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.eulerAngles += new Vector3(0.0f, -0.025f, 0.0f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.eulerAngles += new Vector3(0.0f, 0.025f, 0.0f);
        }
    }
}
