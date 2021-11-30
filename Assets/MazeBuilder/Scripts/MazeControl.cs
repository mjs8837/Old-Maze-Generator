using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeControl : MonoBehaviour
{
    public Text sizeText;
    public GameObject wallMaster;
    public GameObject ShortGoals;
    public GameObject LongGoals;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp)) // Scale up
        {
            this.transform.localScale = this.transform.localScale + new Vector3(.025f, 0, .025f);
            sizeText.text = "StageSize" + this.transform.localScale.x.ToString();
        }
        if (Input.GetKeyDown(KeyCode.PageDown)) // Scale down
        {
            this.transform.localScale = this.transform.localScale - new Vector3(.025f, 0, .025f);
        }
        if (Input.GetKeyDown(KeyCode.Home)) // Toggle walls
        {
            wallMaster.SetActive(wallMaster.activeSelf);
            sizeText.gameObject.SetActive(!wallMaster.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Insert)) // Change goal
        {
            if (ShortGoals.activeSelf)
            {
                ShortGoals.SetActive(false);
                LongGoals.SetActive(true);
            }
            else
            {
                ShortGoals.SetActive(true);
                LongGoals.SetActive(false);
            }
            wallMaster.SetActive(false);
            sizeText.gameObject.SetActive(true);
        }
    }
}
